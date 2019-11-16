using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    public class DefaultDataMigration<T> : IDataMigration
    {
        private readonly ISource<T> _source;
        private readonly IDestination<T> _destination;
        private readonly Settings _settings;
        private readonly IDictionary<string, string> _sourceParameters;
        private readonly IDictionary<string, string> _destinationParameters;

        public DefaultDataMigration(
            ISource<T> source, 
            IDestination<T> destination, 
            Settings settings,
            IDictionary<string, string> sourceParameters,
            IDictionary<string, string> destinationParameters)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _destination = destination ?? throw new ArgumentNullException(nameof(destination));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _sourceParameters = sourceParameters ?? throw new ArgumentNullException(nameof(sourceParameters));
            _destinationParameters = destinationParameters ?? throw new ArgumentNullException(nameof(destinationParameters));
        }

        public async Task StartAsync()
        {
            await InternalStart();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }

        private Task InternalStart()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            Console.WriteLine("InternalStart...");
            new TaskFactory().StartNew(async () =>
            {
                Console.WriteLine("In Task...");
                Exception exception = null;
                try
                {
                    await this._source.PrepareAsync(this._sourceParameters);
                    await this._destination.PrepareAsync(this._destinationParameters);

                    do
                    {
                        Console.WriteLine("Before Source.GetAsync");
                        var items = await this._source.GetAsync(_settings.BatchSize);
                        items = items.ToList();
                        Console.WriteLine($"Items length:{items.Count()}");
                        if (!items.Any())
                        {
                            Console.WriteLine("Done...");
                            break;
                        }

                        await this._destination.ConsumeAsync(items);
                    } while (true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    exception = e;
                }
                finally
                {
                    await this._source.CleanupAsync();
                    await this._destination.CleanupAsync();
                    if (exception != null)
                    {
                        tcs.SetException(exception);
                    }
                    else
                    {
                        tcs.SetResult(0);
                    }
                }
            });

            return tcs.Task;
        }
    }
}