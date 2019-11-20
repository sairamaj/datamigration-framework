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
        /// Gets data migration reference if one already exists otherwise creates new one.
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
        IDataMigration Get(Guid id, string migrationTaskName, IDictionary<string, string> parameters);
    }
}