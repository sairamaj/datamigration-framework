using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    /// <summary>
    /// Source for reading persons data file.
    /// </summary>
    public class PersonFileSource : ISource<Person>
    {
        private StreamReader _sr;

        public Task PrepareAsync(IDictionary<string, string> parameters)
        {
            var fileName = parameters["inputFileName"];
            _sr = new StreamReader(File.OpenRead(fileName));
            return Task.FromResult(0);
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

        public Task CleanupAsync()
        {
            _sr?.Close();
            return Task.FromResult(0);
        }
    }
}
