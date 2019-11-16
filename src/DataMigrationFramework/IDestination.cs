using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    public interface IDestination<T>
    {
        Task PrepareAsync(IDictionary<string, string> parameters);
        Task ConsumeAsync(IEnumerable<T> items);
        Task CleanupAsync();
    }
}
