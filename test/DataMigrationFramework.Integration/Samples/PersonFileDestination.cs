using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DataMigrationFramework.Integration.Model;

namespace DataMigrationFramework.Integration.Samples
{
    public class PersonFileDestination :IDestination<Person>
    {
        private readonly string _outputFile = @"TestFile\personsdestination.txt";
        StreamWriter _sw;

        public Task PrepareAsync()
        {
            _sw = new StreamWriter(this._outputFile);
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
        }

        public async Task ConsumeAsync(IEnumerable<Person> items)
        {
            foreach (var item in items)
            {
                Console.WriteLine(item);
                await _sw.WriteLineAsync($"{item.Name},{item.Age}");
            }
        }

        public Task CleanupAsync()
        {
            _sw?.Close();
            var tcs = new TaskCompletionSource<int>();
            tcs.SetResult(0);
            return tcs.Task;
        }
    }
}
