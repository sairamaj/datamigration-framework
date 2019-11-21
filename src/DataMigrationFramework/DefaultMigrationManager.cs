using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

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
        /// Creates data migration reference if one does not exists, otherwise creates new one.
        /// </summary>
        /// <param name="id">
        /// Id of the migration.
        /// </param>
        /// <param name="migrationTaskName">
        /// Migration task name.
        /// </param>
        /// <param name="parameters">
        /// Task parameters.
        /// </param>
        /// <returns>
        /// A <see cref="IDataMigration"/> instance. Either a newly created one if does not exist or previously created one.
        /// </returns>
        public IDataMigration Create(Guid id, string migrationTaskName, IDictionary<string, string> parameters)
        {
            if (this._dataMigrationsMap.TryGetValue(id, out IDataMigration val))
            {
                return val;
            }

            var migration = this._factory.Get(id, migrationTaskName, parameters);
            this._dataMigrationsMap[id] = migration;
            return migration;
        }

        /// <summary>
        /// Gets existing data migration if one found.
        /// </summary>
        /// <param name="id">
        /// Unique identifier.
        /// </param>
        /// <returns>
        /// A <see cref="IDataMigration"/> instance if found, otherwise null.
        /// </returns>
        public IDataMigration Get(Guid id)
        {
            if (this._dataMigrationsMap.TryGetValue(id, out IDataMigration val))
            {
                return val;
            }

            return null;
        }
    }
}
