using DataMigrationFramework.Exceptions;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Collects the status of the migration process.
    /// </summary>
    internal class StatusCollector
    {
        /// <summary>
        /// Settings used for controlling.
        /// </summary>
        private readonly Settings _settings;

        /// <summary>
        /// Previous value.
        /// </summary>
        private int _previousDivision;

        /// <summary>
        /// Total produced.
        /// </summary>
        private int _totalProduced;

        /// <summary>
        /// Total produced.
        /// </summary>
        private int _totalConsumed;

        /// <summary>
        /// Total errors count.
        /// </summary>
        private int _totalErrors;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusCollector"/> class.
        /// </summary>
        /// <param name="settings">
        /// A <see cref="Settings"/> used for controlling the process.
        /// </param>
        public StatusCollector(Settings settings)
        {
            this._settings = settings;
        }

        /// <summary>
        /// Gets total produced records count.
        /// </summary>
        public int TotalProduced => this._totalProduced;

        /// <summary>
        /// Gets total consumed records count.
        /// </summary>
        public int TotalConsumed => this._totalConsumed;

        /// <summary>
        /// Gets total errors count.
        /// </summary>
        public int TotalErrors => this._totalErrors;

        /// <summary>
        /// Gets a value indicating whether status should be notified or not.
        /// </summary>
        public bool IsStatusNotify { get; private set; }

        /// <summary>
        /// Updates record counts.
        /// </summary>
        /// <param name="currentProduced">
        /// Current produced record count.
        /// </param>
        /// <param name="currentConsumed">
        /// Current consumed record count.
        /// </param>
        /// <param name="errorCount">
        /// Current error count.
        /// </param>
        public void Update(int currentProduced, int currentConsumed, int errorCount)
        {
            this._totalErrors += errorCount;
            this._totalProduced += currentProduced;
            this._totalConsumed += currentConsumed;

            if (this.TotalProduced >= this._settings.MaxNumberOfRecords)
            {
                throw new MaxLimitReachedException(
                    $"Max threshold reached and hence exiting.",
                    this._totalProduced,
                    this._settings.MaxNumberOfRecords);
            }

            if (this._totalErrors >= this._settings.ErrorThresholdBeforeExit)
            {
                throw new ErrorThresholdReachedException(
                    $"Error threshold reached and hence exiting.",
                    this._totalErrors,
                    this._settings.ErrorThresholdBeforeExit);
            }

            var curDivision = this._totalProduced / this._settings.NotifyStatusRecordSizeFrequency;
            if (curDivision <= this._previousDivision)
            {
                this.IsStatusNotify = false;
            }
            else
            {
                this._previousDivision++;
                this.IsStatusNotify = true;
            }
        }
    }
}