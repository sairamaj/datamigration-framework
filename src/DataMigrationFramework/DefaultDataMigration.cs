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
        /// Flag for keeping disposed or not.
        /// </summary>
        private bool _disposed;

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
            this.CurrentStatus = MigrationStatus.NotStarted;
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
        /// Gets current migration information.
        /// </summary>
        private MigrationInformation CurrentMigrationInformation => new MigrationInformation(this.Id, this.CurrentStatus, this._parameters)
        {
            LastException = this.LastException,
            TotalErrorCount = this._statusCollector.TotalErrors,
            TotalRecordsProduced = this._statusCollector.TotalProduced,
            TotalRecordsConsumed = this._statusCollector.TotalConsumed,
        };

        /// <summary>
        /// Starts the migration process.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> object representing asynchronous operation. A <see cref="MigrationStatus"/> will be returned as part of task object.
        /// </returns>
        public async Task<MigrationInformation> StartAsync()
        {
            this.Validate();
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
        /// Disposes the used resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts the migration process.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation.
        /// </returns>
        private Task<MigrationInformation> InternalStart()
        {
            TaskCompletionSource<MigrationInformation> tcs = new TaskCompletionSource<MigrationInformation>();
            new TaskFactory().StartNew(
                async () =>
            {
                try
                {
                    this.FlagStatus(MigrationStatus.Starting);
                    this.FlagStatus(MigrationStatus.Running);
                    this.CurrentMigrationInformation.DateStart = DateTime.Now;
                    await this._source.PrepareAsync(this._parameters);
                    await this._destination.PrepareAsync(this._parameters);

                    do
                    {
                        var producerHelper = new ProducerHelper<T>(this._source, this._settings.NumberOfProducers);
                        var items = (await producerHelper.ProduceAsync(this._settings.BatchSize, this._cancellationToken.Token)).ToList();
                        int currentProduced = items.Count();
                        if (currentProduced == 0)
                        {
                            break;
                        }

                        var consumerHelper = new ConsumerHelper<T>(this._destination, this._settings.NumberOfConsumers);
                        var successCount = await consumerHelper.ConsumeAsync(items, this._cancellationToken.Token);

                        this._statusCollector.Update(currentProduced, successCount, currentProduced - successCount);
                        if (this._statusCollector.IsStatusNotify)
                        {
                            this.Notify();
                        }

                        await Task.Delay(this._settings.DelayBetweenBatches, this._cancellationToken.Token);
                    }
                    while (true);

                    this.FlagStatus(MigrationStatus.Completed);
                }
                catch (OperationCanceledException)
                {
                    this.FlagStatus(MigrationStatus.Cancelled);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    this.LastException = e;
                    this.FlagStatus(MigrationStatus.Exception);
                }
                finally
                {
                    this.CurrentMigrationInformation.DateEnd = DateTime.Now;
                    await this._source.CleanupAsync(this.CurrentStatus);
                    await this._destination.CleanupAsync(this.CurrentStatus);
                    this.Dispose();
                    tcs.SetResult(this.CurrentMigrationInformation);
                }
            },
                TaskCreationOptions.LongRunning);

            return tcs.Task;
        }

        /// <summary>
        /// Validates the settings before start.
        /// </summary>
        private void Validate()
        {
            if (this._settings.NumberOfConsumers < 1 || this._settings.NumberOfConsumers > 32)
            {
                throw new InvalidOperationException($"{this._settings.NumberOfConsumers} is not valid. It should be between 1-32");
            }
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
            this._monitor.Notify(this.CurrentMigrationInformation);
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        /// <param name="isDisposing">
        /// Flag to indicate whether it is coming through dispose.
        /// </param>
        private void Dispose(bool isDisposing)
        {
            if (isDisposing && !this._disposed)
            {
                this._cancellationToken?.Dispose();
                this._monitor.Dispose();
                this._disposed = true;
            }
        }
    }
}