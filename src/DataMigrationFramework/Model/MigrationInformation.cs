using System;
using System.Collections.Generic;
using System.Text;

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
        public MigrationInformation(Guid id, MigrationStatus status)
        {
            this.Id = id;
            this.Status = status;
        }

        /// <summary>
        /// Gets migration id.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets migration status.
        /// </summary>
        public MigrationStatus Status { get; }
    }
}