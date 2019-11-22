namespace DataMigrationFramework.Model
{
    /// <summary>
    /// Represents the migration status.
    /// </summary>
    public enum MigrationStatus
    {
        /// <summary>
        /// Not started status.
        /// </summary>
        NotStarted,

        /// <summary>
        /// Starting the process.
        /// </summary>
        Starting,

        /// <summary>
        /// Running state.
        /// </summary>
        Running,

        /// <summary>
        /// Completed status.
        /// </summary>
        Completed,

        /// <summary>
        /// Cancelled status.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Exception status.
        /// </summary>
        Exception,
    }
}