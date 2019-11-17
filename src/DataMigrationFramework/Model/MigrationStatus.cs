namespace DataMigrationFramework.Model
{
    public enum MigrationStatus
    {
        NotStarted,
        InSource,
        InDestination,
        Paused,
        Completed,
        Cancelled,
        Exception
    }
}
