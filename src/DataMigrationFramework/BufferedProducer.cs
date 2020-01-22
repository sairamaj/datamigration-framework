using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void Stop()
        {
            this._stopReadingEvent.Set();
        }

        public void Pause()
        {
            this._producerTracer.Log(TraceName, "Pausing");
            this._recordTracker.Pause();
        }

        public void Continue()
        {
            this._producerTracer.Log(TraceName, "Continue");
            this._recordTracker.Continue();
        }

        public IEnumerable<T> Get(int batchSize)
        {
            var watch = new Stopwatch();
            watch.Start();
            while (true)
            {
                this._producerTracer.Log(TraceName, $"[Get]Looking for Current:{this.CurrentCachedRecordCount} : {batchSize} : {this._doneReadingRecords}");
                if (this.CurrentCachedRecordCount > 0)
                {
                    watch.Stop();
                    this._producerTracer.Log(TraceName, $"Reading records now as we have records...:waited:{watch.ElapsedMilliseconds}(ms)");
                    return this.TryTakeAndRemove(batchSize);     // we have enough records in cache to return.
                }

                if (this._doneReadingRecords)
                {
                    watch.Stop();
                    this._producerTracer.Log(TraceName, $"DoneReadingRecords. Take whatever we have: {watch.ElapsedMilliseconds}(ms).");
                    return this.TryTakeAndRemove(batchSize);     // we have enough records in cache to return.
                }

                // we don't have enough. lets wait for records ready.
                var waitForRecords = new WaitHandle[2];
                waitForRecords[0] = this._recordsReadAndReady;
                waitForRecords[1] = this._doneWithReadingEvent;

                this._producerTracer.Log(TraceName, $"[Get]Waiting for records loop...");
                var ret = WaitHandle.WaitAny(waitForRecords, TimeWaitForRecordsReady);
                this._producerTracer.Log(TraceName, $"[Get]ret:{ret} _doneReadingRecords:{this._doneReadingRecords}");
                if (this._engineException != null)
                {
                    throw this._engineException;
                }
                this._producerTracer.Log(TraceName, $"waited: {watch.ElapsedMilliseconds}(ms).");
            }
        }

        void ReaderThread(object val)
        {
            try
            {
                while (true)
                {
                    this._recordTracker.Reset();
                    var recordsRead = this._recordsProducer(this._batchSize).ToList();
                    this._producerTracer.Log(TraceName, $"[ReaderThread] records read:{recordsRead.Count()}");

                    this._producerTracer.Log(TraceName, "[ReaderThread] AddRecords");
                    this.AddRecords(recordsRead);
                    this._producerTracer.Log(TraceName, "[ReaderThread] Setting _recordsReadAndReady");
                    this._recordsReadAndReady.Set();

                    if (recordsRead.Count() < this._batchSize || this._stopReadingEvent.WaitOne(0))
                    {
                        this._doneReadingRecords = true;
                        this._doneWithReadingEvent.Set();
                        this._producerTracer.Log(TraceName, $"[ReaderThread]Records are done now..");
                        break;
                    }

                    if (!this.WaitForThrottlingRecords())
                    {
                        this._producerTracer.Log(TraceName, "[ReaderThread] WaitForThrottlingRecord gave false.");
                        break;  // we are done.
                    }
                }
            }
            catch (Exception e)
            {
                this._engineException = e;       // this will be used to propagate the exception to caller.
                this._producerTracer.Log(TraceName, $"[ReaderThread] Exception:{e}");
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

        private IEnumerable<T> TryTakeAndRemove(int batchSize)
        {
            lock (this._cachedRecordSyncObject)
            {
                // caller is already locked this.
                var records = this._cachedRecords.Take(batchSize).ToArray();
                this._cachedRecords.RemoveRange(0, records.Length);
                if (records.Any())
                {
                    this._recordsReadAndReady.Set();        // we have some more records
                }

                this._recordThrottlingEvent.Set();
                return records;
            }
        }
    }
}
