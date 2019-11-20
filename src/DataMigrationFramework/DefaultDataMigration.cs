using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Default data migration implementation of <see cref="IDataMigration"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Type name corresponding to the data migration model which is shared by the <see cref="ISource{T}"/> and <see cref="IDestination{T}"/>.
    /// </typeparam>
    public class DefaultDataMigration<T> : IDataMigration
    {
        /// <summary>
        /// Migration monitor.
        /// </summary>
        private readonly MigrationMonitor _monitor;

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
        /// Status collector.
        /// </summary>
        private readonly StatusCollector _statusCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDataMigration{T}"/> class.
        /// </summary>
        /// <param name="id">
        /// Migration identifier.
        /// </param>
        /// <param name="name">
        /// Migration name.
        /// </param>
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
            Guid id,
            string name,
            ISource<T> source,
            IDestination<T> destination,
            Settings settings,
            IDictionary<string, string> parameters)
        {
            this.Id = id;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name cannot be empty or null", nameof(name));
            }

            this.Name = name;
            this._source = source ?? throw new ArgumentNullException(nameof(source));
            this._destination = destination ?? throw new ArgumentNullException(nameof(destination));
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this._parameters = parameters;

            this._cancellationToken = new CancellationTokenSource();
            this._monitor = new MigrationMonitor();
            this._statusCollector = new StatusCollector(this._settings);
        }

        /// <summary>
        /// Gets migration identifier.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets migration name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets current migration status.
        /// </summary>
        public MigrationStatus CurrentStatus { get; private set; }

        /// <summary>
        /// Gets last exception.
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// Starts the migration process.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> object representing asynchronous operation. A <see cref="MigrationStatus"/> will be returned as part of task object.
        /// </returns>
        public async Task<MigrationStatus> StartAsync()
        {
            return await this.InternalStart();
        }

        /// <summary>
        /// Subscribes to the process.
        /// </summary>
        /// <param name="observer">
        /// A <see cref="IObserver{T}"/> instance.
        /// </param>
        /// <returns>
        /// A <see cref="IDisposable"/> where subscribers can unsubscribe.
        /// </returns>
        public IDisposable Subscribe(IObserver<MigrationInformation> observer)
        {
            return this._monitor.Subscribe(observer);
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
            new TaskFactory().StartNew(async () =>
            {
                Exception exception = null;
                this.FlagStatus(MigrationStatus.Running);
                try
                {
                    await this._source.PrepareAsync(this._parameters);
                    await this._destination.PrepareAsync(this._parameters);

                    do
                    {
                        var items = await this._source.ProduceAsync(this._settings.BatchSize);
                        items = items.ToList();
                        int currentProduced = items.Count();
                        if (currentProduced == 0)
                        {
                            break;
                        }

                        var successCount = await this._destination.ConsumeAsync(items);
                        this._statusCollector.Update(currentProduced, currentProduced- successCount);
                        await Task.Delay(this._settings.SleepBetweenMigration, this._cancellationToken.Token);
                    }
                    while (true);

                    this.FlagStatus(MigrationStatus.Completed);
                }
                catch (TaskCanceledException)
                {
                    this.FlagStatus(MigrationStatus.Cancelled);
                }
                catch (Exception e)
                {
                    this.LastException = e;
                    this.FlagStatus(MigrationStatus.Exception);
                    exception = e;
                }
                finally
                {
                    await this._source.CleanupAsync(this.CurrentStatus);
                    await this._destination.CleanupAsync(this.CurrentStatus);
                    if (exception != null)
                    {
                        tcs.SetException(exception);
                    }
                    else
                    {
                        tcs.SetResult(this.CurrentStatus);
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
            this.CurrentStatus = status;
            this.Notify();
        }

        /// <summary>
        /// Notifies to the observers.
        /// </summary>
        private void Notify()
        {
            this._monitor.Notify(new MigrationInformation(this.Id, this.CurrentStatus)
            {
                LastException = this.LastException,
                CurrentErrorCount = this._statusCollector.TotalErrors,
                TotalRecordsProduced = this._statusCollector.TotalRecords,
            });
        }
    }
}