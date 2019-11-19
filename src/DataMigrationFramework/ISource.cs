using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    public interface ISource<T>
    {
        Task PrepareAsync(IDictionary<string, string> parameters);
        Task<IEnumerable<T>> GetAsync(int batchSize);
        Task CleanupAsync(MigrationStatus state);
    }
}
