using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    public class DefaultDataMigration<T> : IDataMigration
    {
        private readonly ISource<T> _source;
        private readonly IDestination<T> _destination;
        private readonly Settings _settings;
        private readonly CancellationTokenSource _cancellationToken;
        private MigrationStatus _status;

        public DefaultDataMigration(
            ISource<T> source, 
            IDestination<T> destination, 
            Settings settings,
            IDictionary<string, string> parameters)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _destination = destination ?? throw new ArgumentNullException(nameof(destination));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _cancellationToken = new CancellationTokenSource();
            this.Parameters = parameters;
        }

        public IDictionary<string, string> Parameters { get; }

        public async Task<MigrationStatus> StartAsync()
        {
            return await InternalStart();
        }

        public Task StopAsync()
        {
            this._cancellationToken.Cancel();
            var task = new TaskCompletionSource<int>();
            task.SetResult(0);
            return task.Task;
        }

        private Task<MigrationStatus> InternalStart()
        {
            TaskCompletionSource<MigrationStatus> tcs = new TaskCompletionSource<MigrationStatus>();
            Console.WriteLine("InternalStart...");
            new TaskFactory().StartNew(async () =>
            {
                Console.WriteLine("In Task...");
                Exception exception = null;
                try
                {
                    await this._source.PrepareAsync(this.Parameters);
                    await this._destination.PrepareAsync(this.Parameters);

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

                        Console.WriteLine("Consuming Now ...");
                        await this._destination.ConsumeAsync(items);
                        Console.WriteLine($"Pausing for {this._settings.SleepBetweenMigration}");
                        await Task.Delay(this._settings.SleepBetweenMigration, this._cancellationToken.Token);
                    } while (true);

                    this.FlagStatus(MigrationStatus.Completed);
                }
                catch (TaskCanceledException)
                {
                    this.FlagStatus(MigrationStatus.Cancelled);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    this.FlagStatus(MigrationStatus.Completed);
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
                        tcs.SetResult(this._status);
                    }
                }
            });

            return tcs.Task;
        }

        private void FlagStatus(MigrationStatus status)
        {
            this._status = status;
        }
    }
}