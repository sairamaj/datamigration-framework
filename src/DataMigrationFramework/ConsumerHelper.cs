using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    internal class ConsumerHelper<T>
    {
        private readonly IDestination<T> _destination;
        private readonly int _numberOfConsumers;

        public ConsumerHelper(IDestination<T> destination, int numberOfConsumers)
        {
            _destination = destination;
            _numberOfConsumers = numberOfConsumers;
        }

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
            var skip = 0;
            var tasks = new List<Task<int>>();
            for (var i = 0; i < actualConsumers; i++)
            {
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
