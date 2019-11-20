using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace DataMigrationFramework.Unit.Test
{
    [TestFixture()]
    public class DefaultMigrationManagerTest
    {
        [Test]
        public void CtorWithNullFactoryShouldThrowException()
        {
            Action ctorWithNullFactory = () => new DefaultMigrationManager(null);
            ctorWithNullFactory.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: factory");
        }

        [Test]
        public async Task StartAsyncMigrationAlreadyRunningShouldGiveException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockFactory = MockRepository.GenerateMock<IMigrationFactory>();
            var mockDataMigration = MockRepository.GenerateMock<IDataMigration>();
            var parameters = new Dictionary<string, string>();
            mockFactory.Stub(factory => factory.Get("test", parameters)).Return(mockDataMigration);
            mockDataMigration.Stub(migration => migration.StartAsync(id)).Return(Task.FromResult(MigrationStatus.Running));
            mockDataMigration.Stub(migration => migration.CurrentStatus).Return(MigrationStatus.Running);
            var manager = new DefaultMigrationManager(mockFactory);
            Func<Task> firstTask = async () =>
            {
                await manager.StartAsync(id, "test", parameters);
            };
            await firstTask();

            // Act
            Func<Task> startWithAlreadyRunning = async () =>
            {
                await manager.StartAsync(id, "test", parameters);
            };

            // Assert
            startWithAlreadyRunning.Should().Throw<InvalidOperationException>().WithMessage($"{id} is already running.");
        }

        [Test]
        public async Task StartAsyncShouldCallDataMigrationStartAsync()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockFactory = MockRepository.GenerateMock<IMigrationFactory>();
            var mockDataMigration = MockRepository.GenerateMock<IDataMigration>();
            var parameters = new Dictionary<string, string>();
            mockFactory.Stub(factory => factory.Get("test", parameters)).Return(mockDataMigration);
            mockDataMigration.Stub(migration => migration.StartAsync(id)).Return(Task.FromResult(MigrationStatus.Running));
            mockDataMigration.Stub(migration => migration.CurrentStatus).Return(MigrationStatus.Running);
            var manager = new DefaultMigrationManager(mockFactory);

            // Act
            Func<Task> startTask = async () =>
            {
                await manager.StartAsync(id, "test", parameters);
            };
            await startTask();

            // Assert
            mockDataMigration.AssertWasCalled(migration => migration.StartAsync(id));
        }
    }
}
