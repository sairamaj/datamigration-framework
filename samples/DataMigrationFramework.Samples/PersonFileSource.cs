using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    /// <summary>
    /// Source for reading persons data file.
    /// </summary>
    public class PersonFileSource : ISource<Person>
    {
        private readonly IDataAccess _dataAccess;

        public PersonFileSource(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public Task PrepareAsync(IDictionary<string, string> parameters)
        {
            return Task.FromResult(0);
        }

        public async Task<IEnumerable<Person>> ProduceAsync(int batchSize)
        {
            return await this._dataAccess.GetAsync(batchSize);
        }

        public Task CleanupAsync(MigrationStatus status)
        {
            this._dataAccess.Close();
            return Task.FromResult(0);
        }
    }
}
