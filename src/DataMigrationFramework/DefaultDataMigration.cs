using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Default data migration implementation of <see cref="IDataMigration  "/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DefaultDataMigration<T> : IDataMigration
    {
        /// <summary>
        /// Data migration source.
        /// </summary>
        private readonly ISource<T> _source;

        /// <summary>
        /// Data migration destination.
        /// </summary>
        private readonly IDestination<T> _destination;

        /// <summary>
        /// Settings used for migration.
        /// </summary>
        private readonly Settings _settings;

        /// <summary>
        /// Cancellation token used for cancelling the running migration process.
        /// </summary>
        private readonly CancellationTokenSource _cancellationToken;

        /// <summary>
        /// Runtime parameters for source and destination.
        /// </summary>
        private readonly IDictionary<string, string> _parameters;

        /// <summary>
        /// Current migration status.
        /// </summary>
        private MigrationStatus _currentStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataMigration{T}"/> class.
        /// </summary>
        /// <param name="source">
        /// A <see cref="ISource{T}"/> implementation of the source interface for the data.
        /// </param>
        /// <param name="destination">
        /// A <see cref="IDestination{T}"/> implementation of the destination for the data.
        /// </param>
        /// <param name="settings">
        /// A <see cref="Settings"/> used for migration.
        /// </param>
        /// <param name="parameters">
        /// A <see cref="IDictionary{TKey,TValue}"/> parameters passed to source and destination references for runtime parameters.
        /// </param>
        public DefaultDataMigration(
            ISource<T> source, 
            IDestination<T> destination, 
            Settings settings,
            IDictionary<string, string> parameters)
        {
            this._source = source ?? throw new ArgumentNullException(nameof(source));
            this._destination = destination ?? throw new ArgumentNullException(nameof(destination));
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._cancellationToken = new CancellationTokenSource();
            this._parameters = parameters;
        }


        /// <summary>
        /// Start the data migration process.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> representing asynchronous operation.
        /// </returns>
        public async Task<MigrationStatus> StartAsync()
        {
            return await InternalStart();
        }

        /// <summary>
        /// Stops the running migration process.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> representing asynchronous operation.
        /// </returns>
        public Task StopAsync()
        {
            this._cancellationToken.Cancel();
            var task = new TaskCompletionSource<int>();
            task.SetResult(0);
            return task.Task;
        }

        /// <summary>
        /// Starts the migration process.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation.
        /// </returns>
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
                    await this._source.PrepareAsync(this._parameters);
                    await this._destination.PrepareAsync(this._parameters);

                    do
                    {
                        Console.WriteLine("Before Source.GetAsync");
                        var items = await this._source.ProduceAsync(_settings.BatchSize);
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
                    await this._source.CleanupAsync(this._currentStatus);
                    await this._destination.CleanupAsync(this._currentStatus);
                    if (exception != null)
                    {
                        tcs.SetException(exception);
                    }
                    else
                    {
                        tcs.SetResult(this._currentStatus);
                    }
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// Flags the current migration status.
        /// </summary>
        /// <param name="status">
        /// A <see cref="MigrationStatus"/> status.
        /// </param>
        private void FlagStatus(MigrationStatus status)
        {
            this._currentStatus = status;
        }
    }
}