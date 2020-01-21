using System.Collections.Generic;

namespace DataMigrationFramework
{
    public interface IProducer<T>
    {
        IEnumerable<T> Get(int size);
        void Start();
        void Pause();
        void Continue();
    }
}
