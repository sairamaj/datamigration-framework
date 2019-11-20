using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public class PersonFileDestination :IDestination<Person>
    {
        private readonly IDataAccess _dataAccess;

        public PersonFileDestination(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public Task PrepareAsync(IDictionary<string, string> parameters)
        {
            var fileName = parameters["outputFileName"];
            return Task.FromResult(0);
        }

        public async Task<int> ConsumeAsync(IEnumerable<Person> persons)
        {
            persons = persons.ToList();
            await this._dataAccess.SaveAsync(persons);
            return persons.Count();
        }

        public Task CleanupAsync(MigrationStatus status)
        {
            this._dataAccess.Close();
            return Task.FromResult(0);
        }
    }
}
