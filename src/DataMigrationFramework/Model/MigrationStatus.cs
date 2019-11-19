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
        /// In produce status.
        /// </summary>
        InProduce,

        /// <summary>
        /// In consume status.
        /// </summary>
        InConsume,

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