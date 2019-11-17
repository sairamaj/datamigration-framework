using System.Collections.Generic;
using DataMigrationFramework.Model;

namespace DataMigrationFramework
{
    public interface IMigrationFactory
    {
        IEnumerable<Configuration> GetInfo();
        IDataMigration Get(string name, IDictionary<string, string> parameters);
    }
}
