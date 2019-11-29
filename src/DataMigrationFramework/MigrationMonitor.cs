using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Migration monitor helper class.
    /// </summary>
    internal class MigrationMonitor : IDisposable
    {
        /// <summary>
        /// List of observers for notification.
        /// </summary>
        private readonly List<IObserver<MigrationInformation>> _observers = new List<IObserver<MigrationInformation>>();

        /// <summary>
        /// Un subscribers are maintained to cleanup upon dispose.
        /// </summary>
        private readonly List<UnSubscriber> _unSubscribers = new List<UnSubscriber>();

        /// <summary>
        /// Flag for keeping track of disposed or not.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Subscribe helper.
        /// </summary>
        /// <param name="observer">
        /// A <see cref="IObserver{T}"/> of <see cref="MigrationStatus"/> instance.
        /// </param>
        /// <returns>
        /// A <see cref="IDisposable"/> where one can unsubscribe.
        /// </returns>
        public IDisposable Subscribe(IObserver<MigrationInformation> observer)
        {
            this._observers.Add(observer);
            var unSubscriber = new UnSubscriber(this._observers, observer);

            // add to our list and in case client does not dispose we will dispose through this list.
            this._unSubscribers.Add(unSubscriber);
            return unSubscriber;
        }

        /// <summary>
        /// Notifies the progress to observers.
        /// </summary>
        /// <param name="info">
        /// A <see cref="MigrationInformation"/> info.
        /// </param>
        public void Notify(MigrationInformation info)
        {
            foreach (var observer in this._observers)
            {
                observer.OnNext(info);
            }
        }

        /// <summary>
        /// Disposes the internals.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the unSubscribers.
        /// </summary>
        /// <param name="disposing">
        /// Flag to see that we are disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (disposing && !this._disposed)
            {
                foreach (var unSubscriber in this._unSubscribers)
                {
                    unSubscriber.Dispose();
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// un subscriber utility class.
        /// </summary>
        private class UnSubscriber : IDisposable
        {
            /// <summary>
            /// List of observers.
            /// </summary>
            private readonly List<IObserver<MigrationInformation>> _observers;

            /// <summary>
            /// Observer which will be removed.
            /// </summary>
            private IObserver<MigrationInformation> _observer;

            /// <summary>
            /// Initializes a new instance of the <see cref="UnSubscriber"/> class.
            /// </summary>
            /// <param name="observers">List of observers.</param>
            /// <param name="observer">Observer to be removed.</param>
            public UnSubscriber(List<IObserver<MigrationInformation>> observers, IObserver<MigrationInformation> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            /// <summary>
            /// Disposes method.
            /// </summary>
            public void Dispose()
            {
                this.Dispose(true);
            }

            /// <summary>
            /// Dispose utility.
            /// </summary>
            /// <param name="isDisposing">true if disposing.</param>
            private void Dispose(bool isDisposing)
            {
                Trace.WriteLine($"[memory] MigrationMonitor Dispose.");
                if (isDisposing)
                {
                    if (this._observer != null)
                    {
                        Trace.WriteLine($"[memory] Removing the observer.");
                        this._observers.Remove(this._observer);
                        this._observer = null;
                    }
                }
            }
        }
    }
}