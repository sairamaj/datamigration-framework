using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace DataMigrationFramework.Unit.Test
{
    [TestFixture]
    public class DefaultMigrationManagerTest
    {
        [Test]
        public void CtorWithNullFactoryShouldThrowException()
        {
            Action ctorWithNullFactory = () => new DefaultMigrationManager(null);
            ctorWithNullFactory.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: factory");
        }

        [Test]
        public void GetNonExistingMigrationShouldReturnNull()
        {
            // Arrange
            var mockFactory = MockRepository.GenerateMock<IMigrationFactory>();
            var manager = new DefaultMigrationManager(mockFactory);

            // Act
            var migration = manager.Get(Guid.NewGuid());

            // Assert
            migration.Should().BeNull();
        }

        [Test]
        public void GetExistingMigrationShouldReturnExistingOne()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockFactory = MockRepository.GenerateMock<IMigrationFactory>();
            var manager = new DefaultMigrationManager(mockFactory);
            var mockMigration = MockRepository.GenerateMock<IDataMigration>();
            mockFactory.Stub(factory => factory.Get(id, "test", new Dictionary<string, string>())).Return(mockMigration);
            var migration = manager.Create(id, "test", new Dictionary<string, string>());

            // Act
            var migrationFromGet = manager.Get(id);

            // Assert
            migrationFromGet.Should().Be(migration);
        }

        [Test]
        public void CreateShouldCreateNewMigrationIfOneDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockFactory = MockRepository.GenerateMock<IMigrationFactory>();
            var manager = new DefaultMigrationManager(mockFactory);
            var mockMigration = MockRepository.GenerateMock<IDataMigration>();
            mockFactory.Stub(factory => factory.Get(id, "test", new Dictionary<string, string>())).Return(mockMigration);

            // Act
            var migration = manager.Create(id, "test", new Dictionary<string, string>());

            // Assert
            migration.Should().Be(mockMigration);
        }

        [Test]
        public void CreateShouldReturnExistingMigrationIfOneExistsAlready()
        {
            // Arrange
            var id = Guid.NewGuid();
            var mockFactory = MockRepository.GenerateMock<IMigrationFactory>();
            var manager = new DefaultMigrationManager(mockFactory);
            var mockMigration = MockRepository.GenerateMock<IDataMigration>();
            mockFactory.Stub(factory => factory.Get(id, "test", new Dictionary<string, string>())).Return(mockMigration).Repeat.Once();

            // Act
            var migration1 = manager.Create(id, "test", new Dictionary<string, string>());
            var migration2 = manager.Create(id, "test", new Dictionary<string, string>());

            // Assert
            migration2.Should().Be(migration1);
        }
    }
}