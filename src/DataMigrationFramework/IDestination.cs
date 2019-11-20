using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Destination interface used for implementing data source for migration.
    /// Migration framework uses Consume method to pass the data.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the model where source and destination use.
    /// </typeparam>
    public interface IDestination<T>
    {
        /// <summary>
        /// Prepares the destination before the migration called by the framework.
        /// </summary>
        /// <remarks>
        /// Use this to do one time activity for migration.
        /// </remarks>
        /// <param name="parameters">
        /// A <see cref="IDictionary{TKey,TValue}"/> parameters passed from the data migration.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> object representing the async operation.
        /// </returns>
        Task PrepareAsync(IDictionary<string, string> parameters);

        /// <summary>
        /// Consuming the migration model items.
        /// </summary>
        /// <param name="items">
        /// A <see cref="IEnumerable{T}"/> of migration model.
        /// </param>
        /// <returns>
        /// The <see cref="Task{TREsult}"/> object representing the asynchronous operation.
        /// </returns>
        Task<int> ConsumeAsync(IEnumerable<T> items);

        /// <summary>
        /// Cleanup done after migration.
        /// </summary>
        /// <remarks>
        /// Called by the framework when migration is done one time. Use this method to do any cleanup operation.
        /// </remarks>
        /// <param name="state">
        /// A <see cref="MigrationStatus"/> status representing the status.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> object representing the asynchronous operation.
        /// </returns>
        Task CleanupAsync(MigrationStatus state);
    }
}
