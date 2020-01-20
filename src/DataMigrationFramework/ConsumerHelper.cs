using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    /// <summary>
    /// Helper class to distribute items to consume.
    /// </summary>
    /// <typeparam name="T">
    /// Type name of the model.
    /// </typeparam>
    internal class ConsumerHelper<T>
    {
        /// <summary>
        /// Destination instance.
        /// </summary>
        private readonly IDestination<T> _destination;

        /// <summary>
        /// Number of consumers.
        /// </summary>
        private readonly int _numberOfConsumers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerHelper{T}"/> class.
        /// </summary>
        /// <param name="destination">
        /// A <see cref="IDestination{T}"/> instance.
        /// </param>
        /// <param name="numberOfConsumers">
        /// Number of consumers.
        /// </param>
        public ConsumerHelper(IDestination<T> destination, int numberOfConsumers)
        {
            if (numberOfConsumers <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfConsumers), numberOfConsumers, "Number of consumers should be > 0");
            }

            this._destination = destination ?? throw new ArgumentNullException(nameof(destination));
            this._numberOfConsumers = numberOfConsumers;
        }

        /// <summary>
        /// Consume given items.
        /// </summary>
        /// <param name="items">
        /// A <see cref="IEnumerable{T}"/> of items to pass it to destination consumer.
        /// </param>
        /// <returns>
        /// Actual items consumed successfully.
        /// </returns>
        public int Consume(IEnumerable<T> items)
        {
            items = items.ToList();
            var size = items.Count();
            if (size == 0)
            {
                return 0;
            }

            var actualConsumers = this._numberOfConsumers;
            if (this._numberOfConsumers >= size)
            {
                actualConsumers = size;
            }

            var perConsumerSize = size / actualConsumers;
            var lastSize = size % actualConsumers;
            var skip = 0;
            var tasks = new List<Task<int>>();
            for (var i = 0; i < actualConsumers; i++)
            {
                if (i == actualConsumers-1)
                {
                    perConsumerSize += lastSize;
                }

                Console.WriteLine($"skip {skip} perConsumerSize:{perConsumerSize}");
                var consumerItems = items.Skip(skip).Take(perConsumerSize);
                tasks.Add(this._destination.ConsumeAsync(consumerItems));
                skip += perConsumerSize;
            }

            Console.WriteLine("tasks running...");
            var successCount = 0;
            foreach (var task in tasks)
            {
                Console.WriteLine($" task:");
                successCount += task.Result;
                Console.WriteLine($"success after: {successCount}");
            }

            return successCount;
        }
    }
}