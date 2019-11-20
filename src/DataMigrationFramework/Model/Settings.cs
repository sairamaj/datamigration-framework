namespace DataMigrationFramework.Model
{
    /// <summary>
    /// Settings used for migration process.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Record count frequency for notifications.
        /// </summary>
        private int _notifyStatusRecordSizeFrequency;

        /// <summary>
        /// Record count limit.
        /// </summary>
        private int _maximumRecordsCountLimit;

        /// <summary>
        /// Gets default settings.
        /// </summary>
        public static Settings Default => new Settings()
        {
            BatchSize = 5,
            SleepBetweenMigration = 10,
            ErrorThresholdBeforeExit = 10,
            NotifyStatusRecordSizeFrequency = 100,
            MaxNumberOfRecords = 1000000,
        };

        /// <summary>
        /// Gets or sets batch size which will used to produce and consume.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets sleep time between migration in milliseconds.
        /// </summary>
        public int SleepBetweenMigration { get; set; }

        /// <summary>
        /// Gets or sets number of errors where the migration process stops.
        /// </summary>
        public int ErrorThresholdBeforeExit { get; set; }

        /// <summary>
        /// Gets or sets notify status for every given number of records.
        /// </summary>
        public int NotifyStatusRecordSizeFrequency
        {
            get => this._notifyStatusRecordSizeFrequency <= 0 ? 100 : this._notifyStatusRecordSizeFrequency;
            set => this._notifyStatusRecordSizeFrequency = value;
        }

        /// <summary>
        /// Gets or sets notify status for every given number of records.
        /// </summary>
        public int MaxNumberOfRecords
        {
            get => this._maximumRecordsCountLimit <= 0 ? 1000000 : this._maximumRecordsCountLimit;
            set => this._maximumRecordsCountLimit = value;
        }
    }
}