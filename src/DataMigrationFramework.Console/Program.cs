using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using DataMigrationFramework.Console.Handlers;
using DataMigrationFramework.Console.Requests;
using MediatR;

namespace DataMigrationFramework.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = ServiceContainer.Initialize();
            var mediator = container.Resolve<IMediator>();
            var configurations = mediator.Send(new ListMigrationInfo()).Result;
            foreach (var configuration in configurations)
            {
                System.Console.WriteLine($"{configuration.Name}");
            }

            do
            {
                System.Console.WriteLine("Press any key to continue.");
                System.Console.ReadLine();
                var id = mediator.Send(new StartMigrationRequest("personDataMigration"
                    , new Dictionary<string, string>()
                    {
                        {"inputFileName", @"TestFiles\personsdata.txt" },
                        {"outputFileName", @"TestFiles\personsdataout.txt" }
                    })).Result;
            } while (true);

            //var dataMigration = new Factory(File.ReadAllText("migrationinfo.json"))
            //    .Get(
            //        "personDataMigration",
            //        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){ {"filename", @"TestFiles\personsdata.txt"}},
            //        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { { "filename", @"TestFiles\personsdataout.txt" } });
            //System.Console.WriteLine("Starting...");
            //dataMigration.StartAsync().Wait();
            //System.Console.WriteLine("Waiting to finish...");
        }
    }
}
