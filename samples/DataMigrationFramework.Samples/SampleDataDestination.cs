using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public class SampleDataDestination : IDestination<SampleData>
    {
        public Task PrepareAsync(IDictionary<string, string> parameters)
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
