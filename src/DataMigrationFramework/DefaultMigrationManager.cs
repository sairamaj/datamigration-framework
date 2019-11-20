using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Default implementation of <see cref="IMigrationManager"/> class.
    /// </summary>
    public class DefaultMigrationManager : IMigrationManager
    {
        /// <summary>
        /// Factor for creating migration instances.
        /// </summary>
        private readonly IMigrationFactory _factory;

        /// <summary>
        /// Data migration map.
        /// </summary>
        private readonly IDictionary<Guid, IDataMigration> _dataMigrationsMap = new ConcurrentDictionary<Guid, IDataMigration>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationManager"/> class.
        /// </summary>
        /// <param name="factory">
        /// A <see cref="IMigrationFactory"/> used for creating the migration instances.
        /// </param>
        public DefaultMigrationManager(IMigrationFactory factory)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

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
        public async Task StartAsync(Guid migrationId, string name, IDictionary<string, string> parameters)
        {
            if (!this.CanStart(migrationId))
            {
                throw new InvalidOperationException($"{migrationId} is already running.");
            }

            var migration = this._factory.Get(name, parameters);
            this._dataMigrationsMap[migrationId] = migration;
            await migration.StartAsync(migrationId);
        }

        /// <summary>
        /// Stops existing running migration.
        /// </summary>
        /// <param name="migrationId">
        /// Previously ran migration id.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing asynchronous operation.
        /// </returns>
        public Task StopAsync(Guid migrationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets migration status.
        /// </summary>
        /// <param name="migrationId">
        /// Previously ran migration id.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing asynchronous operation.
        /// </returns>
        public Task<MigrationStatus> GetStatus(Guid migrationId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a migration can start or not.
        /// </summary>
        /// <param name="id">
        /// Id of the migration.
        /// </param>
        /// <returns>
        /// True if can because it does not exist or not running. False if one is already running.
        /// </returns>
        private bool CanStart(Guid id)
        {
            if (!this._dataMigrationsMap.TryGetValue(id, out IDataMigration val))
            {
                // does not exists
                return true;
            }

            return val.CurrentStatus != MigrationStatus.Running;
        }
    }
}
