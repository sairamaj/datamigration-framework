using System;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using FluentAssertions;
using NUnit.Framework;

namespace DataMigrationFramework.Unit.Test
{
    [TestFixture()]
    public class DefaultDataMigrationTest
    {
        [Test]
        public async Task MigrationCompletedNormallyShouldBeInCompletedState()
        {
            // Arrange
            var migration = new DataMigrationCreator().DefaultDataMigration;

            // Act
            var status = await migration.StartAsync();

            // Assert.
            status.Should().Be(MigrationStatus.Completed);
        }

        [Test]
        public async Task MigrationCancelledShouldBeInCancelledState()
        {
            // Arrange
            var migration = new DataMigrationCreator().DefaultDataMigration;

            // Act
            var task = migration.StartAsync();
            await migration.StopAsync();

            var status = await task;
            // Assert.
            status.Should().Be(MigrationStatus.Cancelled);
        }
    }
}
