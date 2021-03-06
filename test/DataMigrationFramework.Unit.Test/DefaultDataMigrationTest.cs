﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        delegate Task<int> ConsumerDelegate<T>(IEnumerable<T> items);

        [Test]
        public async Task MigrationCompletedNormallyShouldBeInCompletedState()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems => Task.FromResult(consumeItems.Count())));
            var migration = creator.DefaultDataMigration;


            // Act
            var result = await migration.StartAsync();

            // Assert.
            result.Status.Should().Be(MigrationStatus.Completed);
        }

        [Test]
        public async Task MigrationCancelledShouldBeInCancelledState()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue, DelayBetweenBatches = 10 };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test" };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items));
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems => Task.FromResult(consumeItems.Count())));
            var migration = creator.DefaultDataMigration;

            // Act
            var task = migration.StartAsync();
            await migration.StopAsync();

            var result = await task;
            // Assert.
            result.Status.Should().Be(MigrationStatus.Cancelled);
        }

        [Test]
        public async Task ErrorLimitReachedShouldStopMigrationWithException()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = 2 };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1", "test2", "test3", "test4", "test5" };
            IEnumerable<string> empty = new List<string>();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            var successCount = 3;
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems => Task.FromResult(successCount)));

            var migration = creator.DefaultDataMigration;


            // Act
            var info = await migration.StartAsync();

            // Assert
            info.Status.Should().Be(MigrationStatus.Exception);
            info.LastException.Should().BeOfType<ErrorThresholdReachedException>();
            info.LastException.Message.Should().Be("Error threshold reached and hence exiting.\r\nErrors: 2 Threshold: 2");
        }

        [Test]
        public async Task MaxLimitReachedShouldStopMigrationWithException()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue, MaxNumberOfRecords = 10 };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1", "test2", "test3", "test4", "test5" };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items));
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems => Task.FromResult(consumeItems.Count())));
            var migration = creator.DefaultDataMigration;

            // Act
            var info = await migration.StartAsync();

            // Assert
            info.Status.Should().Be(MigrationStatus.Exception);
            info.LastException.Should().BeOfType<MaxLimitReachedException>();
            info.LastException.Message.Should().Be("Max threshold reached and hence exiting.\r\nCurrent: 10 MaxLimit: 10");
        }

        [Test]
        public async Task MigrationShouldRaiseNotifications()
        {
            // Arrange+
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems => Task.FromResult(consumeItems.Count())));
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
                MigrationStatus.Starting,
                MigrationStatus.Running,
                MigrationStatus.Completed
            });
        }

        [Test]
        public async Task MigrationShouldRaiseNotificationsFrequentlyBasedOnSetting()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue, NotifyStatusRecordSizeFrequency = 1 };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(items)).Repeat.Times(3);
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems => Task.FromResult(consumeItems.Count())));
            var migration = creator.DefaultDataMigration;

            var states = new List<MigrationStatus>();
            migration.Subscribe((info) =>
            {
                states.Add(info.Status);
            });

            // Act
            await migration.StartAsync();

            // Assert.
            states.Should().Contain(MigrationStatus.Starting);
            states.Should().Contain(MigrationStatus.Running);
            states.Should().Contain(MigrationStatus.Completed);
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
            var result = await migration.StartAsync();

            // Assert.
            result.Status.Should().Be(MigrationStatus.Exception);
            states.Should().BeEquivalentTo(new List<MigrationStatus>
            {
                MigrationStatus.Starting, MigrationStatus.Running, MigrationStatus.Exception
            });
        }

        [Test]
        public void InitialStatusShouldBeInNotStarted()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);

            // Assert
            creator.DefaultDataMigration.CurrentStatus.Should().Be(MigrationStatus.NotStarted);
        }

        [Test]
        public async Task StatusShouldHaveOneStartingStatusWhenMigrationStarted()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            var migration = creator.DefaultDataMigration;

            var startingStatusCount = 0;
            migration.Subscribe((info) =>
            {
                if (info.Status == MigrationStatus.Starting)
                {
                    startingStatusCount++;
                }
            });

            // Act
            await migration.StartAsync();

            // Assert.
            startingStatusCount.Should().Be(1);
        }

        [Test]
        public async Task NumberOfRecordShouldBeAvailableOnCompletion()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 5, ErrorThresholdBeforeExit = Int32.MaxValue, MaxNumberOfRecords = 1000 };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1", "test2", "test3", "test4", "test5" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(5)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(5)).Return(Task.FromResult(empty)).Repeat.Once();
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(items2 => Task.FromResult(3)));
            var migration = creator.DefaultDataMigration;

            var totalProduced = 0;
            var totalConsumed = 0;
            var totalErrors = 0;
            migration.Subscribe((info) =>
            {
                if (info.Status == MigrationStatus.Completed)
                {
                    totalProduced = info.TotalRecordsProduced;
                    totalConsumed = info.TotalRecordsConsumed;
                    totalErrors = info.TotalErrorCount;
                }
            });

            // Act
            await migration.StartAsync();

            // Assert
            totalProduced.Should().Be(5);
            totalConsumed.Should().Be(3);
            totalErrors.Should().Be(2);
        }

        [Test]
        public async Task ObserverThrowingExceptionShouldBeReturnedAsMigrationInformationWithException()
        {
            // Arrange
            var settings = new Settings() { BatchSize = 1, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(1)).Return(Task.FromResult(empty)).Repeat.Once();
            var migration = creator.DefaultDataMigration;

            migration.Subscribe((info) => throw new Exception($"Observer throwing exception..."));

            // Act
            var result = await migration.StartAsync();

            // Assert.
            result.Status.Should().Be(MigrationStatus.Exception);
        }

        [Test(Description = "Multi consumers should be used.")]
        public async Task MultiConsumerShouldBeCreatedWhenNumberOfConsumersSet()
        {
            // Arrange
            var settings = new Settings() { NumberOfConsumers = 3, BatchSize = 3, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1", "test2", "test3" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(3)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(3)).Return(Task.FromResult(empty)).Repeat.Once();
            var consumerCount = 0;
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems =>
                {
                    consumerCount++;            // count consumer count.
                    return Task.FromResult(consumeItems.Count());
                }));
            var migration = creator.DefaultDataMigration;


            // Act
            var result = await migration.StartAsync();

            // Assert.
            consumerCount.Should().Be(3);
            result.Status.Should().Be(MigrationStatus.Completed);
        }

        [Test(Description = "Consumers count is more than produced and should use less consumers.")]
        public async Task NumberOfItemsLessThanNumberOfConsumersShouldUseLessConsumers()
        {
            // Arrange
            var settings = new Settings() { NumberOfConsumers = 10, BatchSize = 3, ErrorThresholdBeforeExit = Int32.MaxValue };
            var creator = new DataMigrationCreator<string>(settings);
            IEnumerable<string> items = new List<string> { "test1", "test2", "test3" };
            IEnumerable<string> empty = new List<string> { };
            creator.MockSource.Stub(source => source.ProduceAsync(3)).Return(Task.FromResult(items)).Repeat.Once();
            creator.MockSource.Stub(source => source.ProduceAsync(3)).Return(Task.FromResult(empty)).Repeat.Once();
            var consumerCount = 0;
            creator.MockDestination.Stub(source => source.ConsumeAsync(items))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(consumeItems =>
                {
                    consumerCount++;            // count consumer count.
                    return Task.FromResult(consumeItems.Count());
                }));
            var migration = creator.DefaultDataMigration;


            // Act
            var result = await migration.StartAsync();

            // Assert.
            consumerCount.Should().Be(3);
            result.Status.Should().Be(MigrationStatus.Completed);
        }
    }
}