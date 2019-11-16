using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    public class DefaultDataMigration<T> : IDataMigration
    {
        private readonly ISource<T> _source;
        private readonly IDestination<T> _destination;

        public DefaultDataMigration(ISource<T> source, IDestination<T> destination)
        {
            _source = source;
            _destination = destination;
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
                    await this._source.PrepareAsync();
                    await this._destination.PrepareAsync();

                    do
                    {
                        Console.WriteLine("Before Source.GetAsync");
                        var items = await this._source.GetAsync(5);
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