using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataMigrationFramework.Exceptions;
using DataMigrationFramework.Model;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace DataMigrationFramework.Unit.Test
{
    [TestFixture]
    public class DefaultDataMigrationTest
    {
        [Test]
        public async Task MigrationCompletedNormallyShouldBeInCompletedState()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> {"test"};
            IEnumerable<string> empty = new List<string> {};
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            creator.MockDestination.Stub(source => source.ConsumeAsync(items)).Return(Task.FromResult(1));
            var migration = creator.DefaultDataMigration;
            

            // Act
            var status = await migration.StartAsync();

            // Assert.
            status.Should().Be(MigrationStatus.Completed);
        }

        [Test]
        public async Task MigrationCancelledShouldBeInCancelledState()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue, SleepBetweenMigration = 10};
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test" };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items));
            creator.MockDestination.Stub(source => source.ConsumeAsync(items)).Return(Task.FromResult(1));
            var migration = creator.DefaultDataMigration;

            // Act
            var task = migration.StartAsync();
            await migration.StopAsync();

            var status = await task;
            // Assert.
            status.Should().Be(MigrationStatus.Cancelled);
        }

        [Test]
        public async Task ErrorLimitReachedShouldStopMigrationWithException()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = 2};
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1", "test2","test3","test4", "test5" };
            IEnumerable<string> empty = new List<string>();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            var successCount = 3;
            creator.MockDestination.Stub(source => source.ConsumeAsync(items)).Return(Task.FromResult(successCount));
            var migration = creator.DefaultDataMigration;


            // Act
            Func<Task<MigrationStatus>> startAsync = async () => await migration.StartAsync();

            // Assert
            await startAsync.Should().ThrowAsync<ErrorThresholdReachedException>().WithMessage("Error threshold reached and hence exiting.\r\nErrors: 2 Threshold: 2");
        }

        [Test]
        public async Task MaxLimitReachedShouldStopMigrationWithException()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue ,MaxNumberOfRecords = 10};
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1", "test2", "test3", "test4", "test5" };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items));
            var successCount = 3;
            creator.MockDestination.Stub(source => source.ConsumeAsync(items)).Return(Task.FromResult(successCount));
            var migration = creator.DefaultDataMigration;

            // Act
            Func<Task<MigrationStatus>> startAsync = async () => await migration.StartAsync();

            // Assert
            await startAsync.Should().ThrowAsync<MaxLimitReachedException>().WithMessage("Max threshold reached and hence exiting.\r\nCurrent: 10 MaxLimit: 10");
        }

        [Test]
        public async Task MigrationShouldRaiseNotifications()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            creator.MockDestination.Stub(source => source.ConsumeAsync(items)).Return(Task.FromResult(1));
            var migration = creator.DefaultDataMigration;

            var states = new List<MigrationStatus>();
            migration.Subscribe((info) =>
            {
                states.Add(info.Status);
            });

            // Act
            await migration.StartAsync();

            // Assert.
            states.Should().BeEquivalentTo(new List<MigrationStatus> { MigrationStatus.Running, MigrationStatus.Completed });
        }

        [Test]
        public async Task MigrationShouldRaiseNotificationsFrequentlyBasedOnSetting()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue, NotifyStatusRecordSizeFrequency = 1};
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Times(3);
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            creator.MockDestination.Stub(source => source.ConsumeAsync(items)).Return(Task.FromResult(1));
            var migration = creator.DefaultDataMigration;

            var states = new List<MigrationStatus>();
            migration.Subscribe((info) =>
            {
                states.Add(info.Status);
            });

            // Act
            await migration.StartAsync();

            // Assert.
            states.Should().BeEquivalentTo(new List<MigrationStatus>
            {
                MigrationStatus.Running,
                MigrationStatus.Running,
                MigrationStatus.Running,
                MigrationStatus.Running,
                MigrationStatus.Completed
            });
        }

        [Test]
        public async Task ShouldRaiseExceptionNotificationsForExceptionEvent()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue, NotifyStatusRecordSizeFrequency = 1 };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1" };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Throw(new Exception("simulated one"));
            creator.MockDestination.Stub(source => source.ConsumeAsync(items)).Return(Task.FromResult(1));
            var migration = creator.DefaultDataMigration;

            var states = new List<MigrationStatus>();
            migration.Subscribe((info) =>
            {
                states.Add(info.Status);
            });

            // Act
            Func<Task> func = async () => await migration.StartAsync();
            await func.Should().ThrowAsync<Exception>();

            // Assert.
            states.Should().BeEquivalentTo(new List<MigrationStatus>
            {
                MigrationStatus.Running,
                MigrationStatus.Exception
            });
        }
    }
}