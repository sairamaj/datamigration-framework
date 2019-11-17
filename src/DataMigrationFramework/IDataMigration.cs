using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Interface defining the data migration.
    /// </summary>
    public interface IDataMigration
    {
        /// <summary>
        /// Starts the migration process.
        /// </summary>
        /// <returns></returns>
        Task<MigrationStatus> StartAsync();

        /// <summary>
        /// Stop the migration process.
        /// </summary>
        /// <returns></returns>
        Task StopAsync();
    }
}
