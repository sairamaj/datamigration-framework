using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Integration.Model;

namespace DataMigrationFramework.Integration.Samples
{
    public class SampleDataSource :ISource<SampleData>
    {
        public Task PrepareAsync()
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
