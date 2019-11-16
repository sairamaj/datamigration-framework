using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationFramework.Integration.Model;

namespace DataMigrationFramework.Integration.Samples
{
    /// <summary>
    /// Source for reading persons data file.
    /// </summary>
    public class PersonFileSource : ISource<Person>
    {
        private readonly string _inputFile = @"TestFile\personsdata.txt";
        private StreamReader _sr;

        public PersonFileSource()
        {
        }

        public Task PrepareAsync()
        {
            _sr = new StreamReader(File.OpenRead(this._inputFile));
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
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
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
        }
    }
}
