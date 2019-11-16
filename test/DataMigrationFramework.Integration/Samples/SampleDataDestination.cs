using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Integration.Model;

namespace DataMigrationFramework.Integration.Samples
{
    public class SampleDataDestination : IDestination<SampleData>
    {
        public Task PrepareAsync()
        {
            throw new NotImplementedException();
        }

        public Task ConsumeAsync(IEnumerable<SampleData> items)
        {
            throw new NotImplementedException();
        }

        public Task CleanupAsync()
        {
            throw new NotImplementedException();
        }
    }
}
