using System;
using System.Collections.Generic;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    /// <summary>
    /// Migration monitor helper class.
    /// </summary>
    internal class MigrationMonitor
    {
        private readonly List<IObserver<MigrationInformation>> _observers = new List<IObserver<MigrationInformation>>();

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
            return new UnSubscriber(this._observers, observer);
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
                try
                {
                    observer.OnNext(info);
                }
                catch (Exception e)
                {
                    // Ignore observers exception.
                    Console.WriteLine(e);
                }
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
                if (isDisposing)
                {
                    if (this._observer != null)
                    {
                        this._observers.Remove(this._observer);
                        this._observer = null;
                    }
                }
            }
        }
    }
}