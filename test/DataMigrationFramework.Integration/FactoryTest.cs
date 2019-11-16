using System;
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
            var dataMigration = new Factory(File.ReadAllText("migrationinfo.json")).Get("personDataMigration");
            Console.WriteLine(dataMigration);
            dataMigration.Should().NotBeNull();
        }

        [Test]
        public void CreateSampleMigration()
        {
            var dataMigration = new Factory(File.ReadAllText("migrationinfo.json")).Get("sampleDataMigration");
            Console.WriteLine(dataMigration);
            dataMigration.Should().NotBeNull();
        }

    }
}
