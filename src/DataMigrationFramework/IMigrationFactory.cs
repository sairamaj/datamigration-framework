using System;
using System.Collections.Generic;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Migration factory for creating the <see cref="IDataMigration"/> instances.
    /// </summary>
    /// <remarks>
    /// Use this for getting migration reference from the JSON configuration.
    /// </remarks>
    public interface IMigrationFactory
    {
        /// <summary>
        /// Gets the current configuration information.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> of <see cref="Configuration"/> representing the data migration configuration.
        /// </returns>
        IEnumerable<Configuration> Configuration { get; }

        /// <summary>
        /// Get data migration reference for the given name.
        /// </summary>
        /// <remarks>
        /// Name is defined in the data migration configuration JSON file.
        /// </remarks>
        /// <param name="id">
        /// Migration identifier.
        /// </param>
        /// <param name="migrationName">
        /// Name of the data migration defined in configuration file.
        /// </param>
        /// <param name="parameters">
        /// Parameters passed to source and destination while creating the data migration.
        /// </param>
        /// <returns>
        /// A <see cref="IDataMigration"/> reference which can be used to start and stop the data migration.
        /// </returns>
        IDataMigration Get(Guid id, string migrationName, IDictionary<string, string> parameters);
    }
}
