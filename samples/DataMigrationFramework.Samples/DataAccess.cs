using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    class DataAccess :IDataAccess
    {
        private StreamReader _sr;

        public DataAccess(string fileName)
        {
            _sr = new StreamReader(File.OpenRead(fileName));
        }

        public async Task<IEnumerable<Person>> GetAsync(int batchSize)
        {
            var persons = new List<Person>();
            for (int i = 0; i < batchSize; i++)
            {
                Console.WriteLine("Read async...");
                var line = await _sr.ReadLineAsync();
                Console.WriteLine($"Read async... {line}");
                if (line == null)
                {
                    break;
                }

                var parts = line.Split(',');
                persons.Add(new Person { Name = parts.First(), Age = Convert.ToInt32(parts.Last()) });
            }

            return persons;
        }

        public void Save(Person p)
        {
            throw new NotImplementedException();
        }
    }
}
