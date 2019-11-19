using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    public interface IDestination<T>
    {
        Task PrepareAsync(IDictionary<string, string> parameters);
        Task ConsumeAsync(IEnumerable<T> items);
        Task CleanupAsync(MigrationStatus state);
    }
}
