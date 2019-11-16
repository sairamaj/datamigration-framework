using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public class SampleDataSource :ISource<SampleData>
    {
        public Task PrepareAsync(IDictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SampleData>> GetAsync(int batchSize)
        {
            throw new NotImplementedException();
        }

        public Task CleanupAsync()
        {
            throw new NotImplementedException();
        }
    }
}
