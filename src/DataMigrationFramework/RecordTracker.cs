using System.Threading;

namespace DataMigrationFramework
{
    class RecordTracker
    {
        private const string TraceName = "RecordTracker";
        private readonly long _topLimit;
        private readonly long _bottomLimit;
        private readonly IProducerTracer _producerTracer;
        private bool _previousMaxReached;
        private int _pauseCount;

        public RecordTracker(
            long topLimit,
            long bottomLimit,
            IProducerTracer producerTracer)
        {
            this._topLimit = topLimit;
            this._bottomLimit = bottomLimit;
            this._producerTracer = producerTracer;
        }

        public bool IsReadyForRead(long currentCount)
        {
            this._producerTracer.Log(TraceName, $"[RecordMaintainer] pauseCount:{this._pauseCount} current: {currentCount} prevmax:{this._previousMaxReached} top:{this._topLimit} bottom:{this._bottomLimit}");

            if (this._pauseCount > 0)
            {
                return false;       // not ready.
            }

            if (currentCount >= this._topLimit)
            {
                this._producerTracer.Log(TraceName, $"\t[RecordMaintainer]Max and cannot read.");
                return false;
            }

            if (currentCount > this._bottomLimit && this._previousMaxReached)
            {
                this._producerTracer.Log(TraceName, $"\t[RecordMaintainer]Previosly reached max but not reached lower yet..");
                return false;
            }

            this._producerTracer.Log(TraceName, $"\t[RecordMaintainer]returning true.");
            return true;
        }

        public void Pause()
        {
           // this._producerTracer.Log(TraceName, "Pause");
            //Interlocked.Increment(ref _pauseCount);
        }

        public void Continue()
        {
            //this._producerTracer.Log(TraceName, "Continue");
            //Interlocked.Decrement(ref _pauseCount);
        }

        public void Set(long currentCount)
        {
            this._producerTracer.Log(TraceName, $"Set: {currentCount}");
            if (currentCount >= this._topLimit)
            {
                this._previousMaxReached = true;
            }
        }

        public void Reset()
        {
            this._producerTracer.Log(TraceName, $"Reset");
            this._previousMaxReached = false;
        }
    }
}
