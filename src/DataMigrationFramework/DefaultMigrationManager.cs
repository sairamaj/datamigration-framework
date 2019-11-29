using System;
using System.Collections;
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
        private readonly LimitedSizeDictionary<Guid, IDataMigration> _dataMigrationsMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationManager"/> class.
        /// </summary>
        /// <param name="factory">
        /// A <see cref="IMigrationFactory"/> used for creating the migration instances.
        /// </param>
        public DefaultMigrationManager(IMigrationFactory factory)
        {
            this._factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this._dataMigrationsMap = new LimitedSizeDictionary<Guid, IDataMigration>(
                100,
                10,
                Comparer<Guid>.Create((x, y) => x.CompareTo(y)));
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
            if (this._dataMigrationsMap.Dictionary.TryGetValue(id, out IDataMigration val))
            {
                return val;
            }

            var migration = this._factory.Get(id, migrationTaskName, parameters);
            this._dataMigrationsMap.Add(new KeyValuePair<Guid, IDataMigration>(id, migration));
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
            if (this._dataMigrationsMap.Dictionary.TryGetValue(id, out IDataMigration val))
            {
                return val;
            }

            return null;
        }

        /// <summary>
        /// Remove the existing.
        /// </summary>
        /// <param name="id">
        /// Unique identifier identifying the existing item.
        /// </param>
        public void Remove(Guid id)
        {
            if (this._dataMigrationsMap.Dictionary.TryGetValue(id, out IDataMigration val))
            {
                this._dataMigrationsMap.Remove(id);
            }
        }
    }
}
