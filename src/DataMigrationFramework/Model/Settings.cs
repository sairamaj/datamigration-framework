namespace DataMigrationFramework.Model
{
    public class Settings
    {
        public int BatchSize { get; set; }
        public int SleepBetweenMigration { get; set; }
        public int ErrorThresholdBeforeExit { get; set; }

        public static Settings Default => new Settings()
        {
            BatchSize = 5,
            SleepBetweenMigration = 10,
            ErrorThresholdBeforeExit = 10
        };
    }
}
