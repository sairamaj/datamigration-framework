using System;
using System.IO;
using System.Threading.Tasks;
using DataMigrationFramework.Integration.Model;
using DataMigrationFramework.Integration.Samples;
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
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFile\personsdestination.txt");
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            var sourceFile =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestFile\personsdata.txt");

            // Act
            await new DefaultDataMigration<Person>(new PersonFileSource(), new PersonFileDestination()).StartAsync();

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
