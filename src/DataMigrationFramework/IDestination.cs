using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataMigrationFramework
{
    public interface IDestination<T>
    {
        Task PrepareAsync();
        Task ConsumeAsync(IEnumerable<T> items);
        Task CleanupAsync();
    }
}
