using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataMigrationFramework.Samples.Model;

namespace DataMigrationFramework.Samples
{
    public class DataAccess :IDataAccess
    {
        private StreamReader _sr;
        private StreamWriter _sw;
        private bool _isDisposed;

        public DataAccess(string inputFileName, string outputFileName)
        {
            Console.WriteLine("In DataAccess.Ctor");
            this._sr = new StreamReader(File.OpenRead(inputFileName));
            this._sw = new StreamWriter(outputFileName);

        }

        ~DataAccess()
        {
            this.Dispose(false);
            GC.SuppressFinalize(this);
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

        public async Task SaveAsync(IEnumerable<Person> persons)
        {
            foreach (var person in persons)
            {
                await _sw.WriteLineAsync($"{person.Name},{person.Age}");
            }
        }

        public void Close()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }



        private void Dispose(bool isDisposing)
        {
            if (isDisposing && !_isDisposed)
            {
                _sw?.Close();
                _sr?.Close();

                this._sw = null;
                this._sr = null;

                this._isDisposed = true;
            }
        }
    }
}
