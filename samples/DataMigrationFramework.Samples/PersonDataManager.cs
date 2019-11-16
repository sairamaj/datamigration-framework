using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public class PersonDataManager
    {
        private readonly string _fileName;

        public PersonDataManager(string fileName)
        {
            _fileName = fileName;
        }

        public IEnumerable<Person> ReadAll()
        {
            foreach (var user in File.ReadAllLines(this._fileName))
            {
                var parts = user.Split(',');
                yield return new Person {Name = parts.First(), Age = Convert.ToInt32(parts.Last())};
            }
        }
    }
}
