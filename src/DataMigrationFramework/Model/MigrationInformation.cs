using System;
using System.Collections.Generic;

namespace DataMigrationFramework.Model
{
    /// <summary>
    /// Migration information.
    /// </summary>
    public class MigrationInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationInformation"/> class.
        /// </summary>
        /// <param name="id">
        /// Migration id.
        /// </param>
        /// <param name="status">
        /// A <see cref="MigrationStatus"/> representing the status of migration.
        /// </param>
        /// <param name="parameters">
        /// A <see cref="IDictionary{TKey,TValue}"/> migration parameters.
        /// </param>
        public MigrationInformation(Guid id, MigrationStatus status, IDictionary<string, string> parameters)
        {
            this.Id = id;
            this.Status = status;
            this.Parameters = parameters;
        }

        /// <summary>
        /// Gets migration id.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets migration status.
        /// </summary>
        public MigrationStatus Status { get; }

        /// <summary>
        /// Gets migration parameters.
        /// </summary>
        public IDictionary<string, string> Parameters { get; }

        /// <summary>
        /// Gets last exception.
        /// </summary>
        public Exception LastException { get; internal set; }

        /// <summary>
        /// Gets total records produced.
        /// </summary>
        public int TotalRecordsProduced { get; internal set; }

        /// <summary>
        /// Gets total records produced.
        /// </summary>
        public int TotalRecordsConsumed { get; internal set; }

        /// <summary>
        /// Gets current error records count.
        /// </summary>
        public int TotalErrorCount { get; internal set; }
    }
}