using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    /// <summary>
    /// Helper class to consolidate from multiple producers.
    /// </summary>
    /// <typeparam name="T">
    /// Type name of the model.
    /// </typeparam>
    internal class ProducerHelper<T>
    {
        /// <summary>
        /// Source instance.
        /// </summary>
        private readonly ISource<T> _source;

        /// <summary>
        /// Number of consumers.
        /// </summary>
        private readonly int _numberOfProducers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerHelper{T}"/> class.
        /// </summary>
        /// <param name="source">
        /// A <see cref="ISource{T}"/> instance for collecting the items.
        /// </param>
        /// <param name="numberOfProducers">
        /// Number of producers.
        /// </param>
        public ProducerHelper(ISource<T> source, int numberOfProducers)
        {
            if (numberOfProducers <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfProducers), numberOfProducers, "Number of producers should be > 0");
            }

            this._source = source ?? throw new ArgumentNullException(nameof(source));
            this._numberOfProducers = numberOfProducers;
        }

        /// <summary>
        /// Consolidates the items from the sources. 
        /// </summary>
        /// <param name="batchSize">
        /// Batch size used for querying the source for number of items.
        /// </param>
        /// <param name="cancellationToken">
        /// Cancellation token used for cancellation.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> of items.
        /// </returns>
        public IEnumerable<T> Produce(int batchSize, CancellationToken cancellationToken)
        {
            var tasks = new List<Task<IEnumerable<T>>>();
            for (var i = 0; i < this._numberOfProducers; i++)
            {
                tasks.Add(this._source.ProduceAsync(batchSize));
            }

            Task.WaitAll(tasks.ToArray(), cancellationToken);

            IList<T> items = new List<T>();
            foreach (var task in tasks)
            {
                items = items.Union(task.Result).ToList();
            }

            return items;
        }
    }
}