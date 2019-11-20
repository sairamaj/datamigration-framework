using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Manages the migration.
    /// </summary>
    public interface IMigrationManager
    {
        /// <summary>
        /// Starts migration with given unique id.
        /// </summary>
        /// <param name="migrationId">
        /// Migration id which can be used for keep tracking the status.
        /// </param>
        /// <param name="name">
        /// Name of the migration task.
        /// </param>
        /// <param name="parameters">
        /// Migration task parameters.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing asynchronous operation.
        /// </returns>
        Task StartAsync(Guid migrationId, string name, IDictionary<string, string> parameters);

        /// <summary>
        /// Stops existing running migration.
        /// </summary>
        /// <param name="migrationId">
        /// Previously ran migration id.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing asynchronous operation.
        /// </returns>
        Task StopAsync(Guid migrationId);

        /// <summary>
        /// Gets migration status.
        /// </summary>
        /// <param name="migrationId">
        /// Previously ran migration id.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing asynchronous operation.
        /// </returns>
        Task<MigrationStatus> GetStatus(Guid migrationId);
    }
}