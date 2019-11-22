using DataMigrationFramework.Model;
using FluentAssertions;
using NUnit.Framework;
using System;
using DataMigrationFramework.Exceptions;

namespace DataMigrationFramework.Unit.Test
{
    [TestFixture]
    public class StatusCollectorTest
    {
        [Test]
        public void StatusNotifyEvaluatorTest1()
        {
            var evaluator = new StatusCollector(new Settings()
            {
                NotifyStatusRecordSizeFrequency = 4,
                ErrorThresholdBeforeExit = 1000,
            });

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();
        }

        [Test]
        public void StatusNotifyEvaluatorTest2()
        {
            var evaluator = new StatusCollector(new Settings()
            {
                NotifyStatusRecordSizeFrequency = 4,
                ErrorThresholdBeforeExit = 1000,
            });

            evaluator.Update(5, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(1, 0, 1);
            evaluator.IsStatusNotify.Should().BeFalse();

            evaluator.Update(10, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();

            evaluator.Update(15, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();

            evaluator.Update(20, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();
        }

        [Test]
        public void StatusNotifyEvaluatorTest3()
        {
            var evaluator = new StatusCollector(new Settings()
            {
                NotifyStatusRecordSizeFrequency = 4,
                ErrorThresholdBeforeExit = 1000,
            });

            evaluator.Update(4, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();
            evaluator.Update(4, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();

            evaluator.Update(4, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();

            evaluator.Update(4, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();

            evaluator.Update(4, 0, 1);
            evaluator.IsStatusNotify.Should().BeTrue();
        }

        [Test]
        public void ErrorLimitReachedShouldThrowException()
        {
            var statusCollector = new StatusCollector(new Settings()
            {
                ErrorThresholdBeforeExit = 10,
            });

            statusCollector.Update(3, 0, 3);
            statusCollector.Update(3, 0, 3);
            statusCollector.Update(3, 0, 3);

            Action updateWhichReachedLimit = () => statusCollector.Update(3, 0, 3);
            updateWhichReachedLimit.Should().Throw<ErrorThresholdReachedException>().WithMessage("Error threshold reached and hence exiting.\r\nErrors: 12 Threshold: 10");
        }
    }
}