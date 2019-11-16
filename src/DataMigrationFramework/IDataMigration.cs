using System;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    /// <summary>
    /// Interface defining the data migration.
    /// </summary>
    interface IDataMigration
    {
        /// <summary>
        /// Starts the migration process.
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// Stop the migration process.
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
    }
}
