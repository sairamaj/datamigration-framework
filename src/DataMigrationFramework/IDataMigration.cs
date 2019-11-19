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
        /// <returns>
        /// A <see cref="Task{T}"/> object representing asynchronous operation. A <see cref="MigrationStatus"/> will be returned as part of task object.
        /// </returns>
        Task<MigrationStatus> StartAsync();

        /// <summary>
        /// Stop the current running migration process.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> object representing asynchronous operation.
        /// </returns>
        Task StopAsync();
    }
}
