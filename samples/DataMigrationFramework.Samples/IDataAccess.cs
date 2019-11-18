using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public interface IDataAccess
    {
        Task<IEnumerable<Person>> GetAsync(int batchSize);
        void Save(Person p);
    }
}
