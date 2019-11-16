using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public class PersonFileDestination :IDestination<Person>
    {
        StreamWriter _sw;

        public Task PrepareAsync(IDictionary<string, string> parameters)
        {
            var fileName = parameters["fileName"];
            _sw = new StreamWriter(fileName);
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
