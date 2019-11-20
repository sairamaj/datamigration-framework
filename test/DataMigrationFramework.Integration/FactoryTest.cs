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
            var dataMigration = new DefaultMigrationFactory(File.ReadAllText("migrationinfo.json"))
                .Get(
                    Guid.NewGuid(),
                    "personDataMigration",
                    new Dictionary<string, string>());
            Console.WriteLine(dataMigration);
            dataMigration.Should().NotBeNull();
        }

        [Test]
        public void CreateSampleMigration()
        {
            var dataMigration = new DefaultMigrationFactory(File.ReadAllText("migrationinfo.json")).Get(
                Guid.NewGuid(),
                "sampleDataMigration",
                    new Dictionary<string, string>());
            Console.WriteLine(dataMigration);
            Console.WriteLine(dataMigration);
            dataMigration.Should().NotBeNull();
        }

    }
}
