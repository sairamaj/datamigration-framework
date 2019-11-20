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
        /// Total count.
        /// </summary>
        private int _totalRecords;

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
        /// Gets total records count.
        /// </summary>
        public int TotalRecords => this._totalRecords;

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
        /// <param name="currentCount">
        /// Current record count.
        /// </param>
        /// <param name="errorCount">
        /// Current error count.
        /// </param>
        public void Update(int currentCount, int errorCount)
        {
            this._totalErrors += errorCount;
            this._totalRecords += currentCount;

            if (this.TotalRecords >= this._settings.MaxNumberOfRecords)
            {
                throw new MaxLimitReachedException(
                    $"Max threshold reached and hence exiting.",
                    this._totalRecords,
                    this._settings.MaxNumberOfRecords);
            }

            if (this._totalErrors >= this._settings.ErrorThresholdBeforeExit)
            {
                throw new ErrorThresholdReachedException(
                    $"Error threshold reached and hence exiting.",
                    this._totalErrors,
                    this._settings.ErrorThresholdBeforeExit);
            }

            var curDivision = this._totalRecords / this._settings.NotifyStatusRecordSizeFrequency;
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