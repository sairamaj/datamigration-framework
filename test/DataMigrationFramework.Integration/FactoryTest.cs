using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace DataMigrationFramework.Integration
{
    public class FactoryTest
    {
        [Test]
        public void CreatePersonMigration()
        {
            var dataMigration = new Factory(File.ReadAllText("migrationinfo.json"))
                .Get(
                    "personDataMigration",
                    new Dictionary<string, string>(),
                    new Dictionary<string, string>());
            Console.WriteLine(dataMigration);
            dataMigration.Should().NotBeNull();
        }

        [Test]
        public void CreateSampleMigration()
        {
            var dataMigration = new Factory(File.ReadAllText("migrationinfo.json")).Get(
                "sampleDataMigration",
                    new Dictionary<string, string>(),
                    new Dictionary<string, string>());
            Console.WriteLine(dataMigration);
            Console.WriteLine(dataMigration);
            dataMigration.Should().NotBeNull();
        }

    }
}
