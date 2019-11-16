using System;
using System.Collections.Generic;
using System.IO;

namespace DataMigrationFramework.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataMigration = new Factory(File.ReadAllText("migrationinfo.json"))
                .Get(
                    "personDataMigration",
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){ {"filename", @"TestFiles\personsdata.txt"}},
                    new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "filename", @"TestFiles\personsdataout.txt" } });
            System.Console.WriteLine("Starting...");
            dataMigration.StartAsync().Wait();
            System.Console.WriteLine("Waiting to finish...");
        }
    }
}
