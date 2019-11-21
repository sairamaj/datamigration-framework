using System;
using System.Collections.Generic;

namespace DataMigrationFramework
{
    /// <summary>
    /// Manages the migration.
    /// </summary>
    public interface IMigrationManager
    {
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
        IDataMigration Create(Guid id, string migrationTaskName, IDictionary<string, string> parameters);

        /// <summary>
        /// Gets existing data migration if one found.
        /// </summary>
        /// <param name="id">
        /// Unique identifier.
        /// </param>
        /// <returns>
        /// A <see cref="IDataMigration"/> instance if found, otherwise null.
        /// </returns>
        IDataMigration Get(Guid id);
    }
}