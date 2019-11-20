using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using DataMigrationFramework.Samples;
using DataMigrationFramework.Samples.Model;
using FluentAssertions;
using NUnit.Framework;

namespace DataMigrationFramework.Integration
{
    [TestFixture]
    public class DefaultDataMigrationTest
    {
        [Test]
        public async Task SuccessfulPersonDataMigrationTest()
        {
            // Arrange
            var outputFile =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\personsdestination.txt");
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var sourceFile =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFiles\personsdata.txt");

            // Act
            var dataAccess = new DataAccess(sourceFile, outputFile);
            await new DefaultDataMigration<Person>(
                new PersonFileSource(dataAccess),
                new PersonFileDestination(dataAccess),
                new Settings()
                {
                    BatchSize = 2,
                    SleepBetweenMigration = 0,
                    ErrorThresholdBeforeExit = 0
                },
                new Dictionary<string, string>
                {
                    {"inputFileName", sourceFile},
                    {"outputFileName", outputFile}
                }
                ).StartAsync(Guid.NewGuid());

            // Assert
            File.Exists(outputFile).Should().BeTrue($"{outputFile} should have existed with migration process");
            new PersonDataManager(outputFile).ReadAll().Should()
                .BeEquivalentTo(
                    new Person { Name = "user1", Age = 1 },
                    new Person { Name = "user2", Age = 2 },
                    new Person { Name = "user3", Age = 3 });
        }
    }
}
