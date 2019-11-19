using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public class SampleDataSource :ISource<SampleData>
    {
        public Task PrepareAsync(IDictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SampleData>> ProduceAsync(int batchSize)
        {
            throw new NotImplementedException();
        }

        public Task CleanupAsync(MigrationStatus state)
        {
            throw new NotImplementedException();
        }
    }
}
