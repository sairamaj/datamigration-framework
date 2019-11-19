using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Source interface used for implementing data source for migration.
    /// Migration framework uses GetAsync method to get data and pass it on to destination consumer.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the model where source and destination use.
    /// </typeparam>
    public interface ISource<T>
    {
        /// <summary>
        /// Prepares the source before the migration called by the framework.
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
        /// Produces the items to be used by the destination.
        /// </summary>
        /// <remarks>
        /// Framework uses this method to send data to destination. Framework stops calling this method once it returns empty list.
        /// </remarks>
        /// <param name="batchSize">
        /// Size of the items to be produced for each call by the producer. Used to limit the resource during the run.
        /// </param>
        /// <returns>
        /// The <see cref="Task{TREsult}"/> object representing the asynchronous operation. The <see cref="Task{T}.Result"/> property returns the list of model objects.
        /// </returns>
        Task<IEnumerable<T>> ProduceAsync(int batchSize);

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
