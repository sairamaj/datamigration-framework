namespace DataMigrationFramework.Model
{
    /// <summary>
    /// Settings used for migration process.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Gets default settings.
        /// </summary>
        public static Settings Default => new Settings()
        {
            BatchSize = 5,
            SleepBetweenMigration = 10,
            ErrorThresholdBeforeExit = 10
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
    }
}
