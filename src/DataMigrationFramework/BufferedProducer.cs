using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataMigrationFramework
{
    public class BufferedProducer<T> : IProducer<T>
    {
        private const string TraceName = "BufferedProducer";
        private readonly Func<int, IEnumerable<T>> _recordsProducer;
        private readonly int _batchSize;
        private readonly IProducerTracer _producerTracer;
        private bool _doneReadingRecords;
        private readonly object _cachedRecordSyncObject = new object();
        private readonly List<T> _cachedRecords = new List<T>();
        readonly AutoResetEvent _recordThrottlingEvent = new AutoResetEvent(false);
        readonly ManualResetEvent _stopReadingEvent = new ManualResetEvent(false);
        readonly AutoResetEvent _recordsReadAndReady = new AutoResetEvent(false);
        readonly ManualResetEvent _doneWithReadingEvent = new ManualResetEvent(false);
        private const int TimeWaitForRecordsReady = 1000 * 5;
        private Exception _engineException;
        private Thread _readerThread;
        private RecordTracker _recordTracker;

        public BufferedProducer(
            Func<int, IEnumerable<T>> recordsProducer,
            int topLimit,
            int bottomLimit,
            int batchSize,
            IProducerTracer producerTracer)
        {
            _recordsProducer = recordsProducer;
            _batchSize = batchSize;
            _producerTracer = producerTracer;
            _recordTracker = new RecordTracker(topLimit, bottomLimit, producerTracer);
        }

        long CurrentCachedRecordCount
        {
            get
            {
                lock (_cachedRecordSyncObject)
                {
                    return _cachedRecords.Count;
                }
            }
        }

        public void Start()
        {
            _readerThread = new Thread(ReaderThread) { IsBackground = true };
            _readerThread.Start();
        }

        public void Pause()
        {
            this._producerTracer.Log(TraceName,"Pausing");
            this._recordTracker.Pause();
        }

        public void Continue()
        {
            this._producerTracer.Log(TraceName, "Continue");
            this._recordTracker.Continue();
        }

        public IEnumerable<T> Get(int batchSize)
        {
            _producerTracer.Log(TraceName, $"[Get]Looking for Current:{CurrentCachedRecordCount} : {batchSize} : {_doneReadingRecords}");
            if (CurrentCachedRecordCount >= batchSize || _doneReadingRecords)
            {
                return TakeAndRemove(batchSize);     // we have enough records in cache to return 
            }

            // we don't have enough. lets wait for records ready.
            while (true)
            {
                var waitForRecords = new WaitHandle[2];
                waitForRecords[0] = _recordsReadAndReady;
                waitForRecords[1] = _doneWithReadingEvent;

                _producerTracer.Log(TraceName, $"[Get]Waiting for records loop...");
                var ret = WaitHandle.WaitAny(waitForRecords, TimeWaitForRecordsReady);

                _producerTracer.Log(TraceName, $"[Get]ret:{ret} _doneReadingRecords:{_doneReadingRecords}");
                if (ret == 0 || _doneReadingRecords)
                {
                    if (_engineException != null)
                    {
                        throw _engineException;
                    }

                    // we have enough size or we are done with reading.
                    return TakeAndRemove(batchSize);
                }
            }
        }


        void ReaderThread(object val)
        {
            try
            {
                while (true)
                {
                    _recordTracker.Reset();
                    var recordsRead = _recordsProducer(_batchSize).ToList();
                    _producerTracer.Log(TraceName, $"[ReaderThread] records read:{recordsRead.Count()}");

                    if (!recordsRead.Any() || _stopReadingEvent.WaitOne(0))
                    {
                        _doneReadingRecords = true;
                        this._doneWithReadingEvent.Set();
                        _producerTracer.Log(TraceName, $"[ReaderThread]Records are done now..");
                        break;
                    }

                    _producerTracer.Log(TraceName, "[ReaderThread] AddRecords");
                    this.AddRecords(recordsRead);
                    _producerTracer.Log(TraceName, "[ReaderThread] Setting _recordsReadAndReady");
                    this._recordsReadAndReady.Set();

                    if (!this.WaitForThrottlingRecords())
                    {
                        _producerTracer.Log(TraceName, "[ReaderThread] WaitForThrottlingRecord gave false.");
                        break;  // we are done.
                    }
                }
            }
            catch (Exception e)
            {
                _engineException = e;       // this will be used to propagate the exception to caller.
                _producerTracer.Log(TraceName, $"[ReaderThread] Exception:{e}");
            }

        }

        private bool WaitForThrottlingRecords()
        {
            do
            {
                _producerTracer.Log(TraceName, $"[WaitForThrottlingRecords]MaxRecords");
                if (!_recordTracker.IsReadyForRead(this.CurrentCachedRecordCount))
                {
                    var waitForRecords = new WaitHandle[2];
                    waitForRecords[0] = _recordThrottlingEvent;
                    waitForRecords[1] = _stopReadingEvent;
                    _producerTracer.Log(TraceName, $"[WaitForThrottlingRecords]Waiting for someone to read.");
                    var ret = WaitHandle.WaitAny(waitForRecords, -1);
                    if (ret == 1)
                    {
                        _producerTracer.Log(TraceName, $"[WaitForThrottlingRecords]returning false as {ret}.");
                        return false; // stopped
                    }
                }
                else
                {
                    break;
                }
            } while (true);

            _producerTracer.Log(TraceName, "[WaitForThrottlingRecords] return true at end.");
            return true;
        }

        void AddRecords(IEnumerable<T> records)
        {
            lock (_cachedRecordSyncObject)
            {
                _cachedRecords.AddRange(records);
                this._recordTracker.Set(this.CurrentCachedRecordCount);
            }
        }

        private IEnumerable<T> TakeAndRemove(int batchSize)
        {
            lock (_cachedRecordSyncObject)
            {
                // caller is already locked this.
                var records = _cachedRecords.Take(batchSize).ToArray();
                _cachedRecords.RemoveRange(0, records.Length);
                _recordThrottlingEvent.Set();
                return records;
            }
        }
    }
}
