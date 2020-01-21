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
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.NumberOfConsumers = 1;
            this.NumberOfProducers = 1;
        }

        /// <summary>
        /// Gets default settings.
        /// </summary>
        public static Settings Default => new Settings()
        {
            NumberOfConsumers = 1,
            NumberOfProducers = 1,
            BatchSize = 5,
            DelayBetweenBatches = 10,
            ErrorThresholdBeforeExit = 10,
            NotifyStatusRecordSizeFrequency = 100,
            MaxNumberOfRecords = 1000000,
        };

        /// <summary>
        /// Gets or sets batch size which will used to produce and consume.
        /// </summary>
        public int BatchSize { get; set; }

        /// <summary>
        /// Gets or sets delay time between batches in milliseconds.
        /// </summary>
        public int DelayBetweenBatches { get; set; }

        /// <summary>
        /// Gets or sets number of errors where the migration process stops.
        /// </summary>
        public int ErrorThresholdBeforeExit { get; set; }

        /// <summary>
        /// Gets or sets number of consumers active.
        /// </summary>
        public int NumberOfConsumers { get; set; }

        /// <summary>
        /// Gets or sets number of producers active.
        /// </summary>
        public int NumberOfProducers { get; set; }
        

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