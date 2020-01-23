using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Rhino.Mocks;

namespace DataMigrationFramework.Unit.Test
{
    [TestFixture]
    public class ConsumerHelperTest
    {
        delegate Task<int> ConsumerDelegate<T>(IEnumerable<T> items);

        [Test(Description = "Ctor with null destination is not allowed.")]
        public void CtorWithNullDestinationShouldThrowException()
        {
            Action ctorWithNullDestination = () => new ConsumerHelper<string>(null, 10);
            ctorWithNullDestination.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: destination");
        }

        [Test(Description = "Number of consumers cannot be zero.")]
        public void CtorWithZeroNumberOfConsumersShouldThrowException()
        {
            Action ctorWithZeroConsumers = () => new ConsumerHelper<string>(MockRepository.GenerateMock<IDestination<string>>(), 0);
            ctorWithZeroConsumers.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Number of consumers should be > 0\r\nParameter name: numberOfConsumers\r\nActual value was 0.");
        }

        [Test(Description = "Number of consumers cannot less than zero.")]
        public void CtorWithNegativeConsumersShouldThrowException()
        {
            Action ctorWithZeroConsumers = () => new ConsumerHelper<string>(MockRepository.GenerateMock<IDestination<string>>(), -1);
            ctorWithZeroConsumers.Should().Throw<ArgumentOutOfRangeException>().WithMessage("Number of consumers should be > 0\r\nParameter name: numberOfConsumers\r\nActual value was -1.");
        }

        [Test(Description = "Zero items should not even try to call destination consum.e")]
        public async Task ConsumeWithZeroItemsShouldNotCallDestinationConsume()
        {
            // Arrange
            var mockDestination = MockRepository.GenerateMock<IDestination<string>>();
            var helper = new ConsumerHelper<string>(mockDestination, 1);

            // Act
            var consumed = await helper.ConsumeAsync(new string[] { }, new CancellationToken());

            consumed.Should().Be(0);
            mockDestination.AssertWasNotCalled(destination => destination.ConsumeAsync(new string[] { }));
        }

        [Test(Description = "With one consumer should call destination consume with same number of parameters.")]
        public async Task ConsumeWithOneConsumerShouldCallWithExactRecords()
        {
            // Arrange
            var mockDestination = MockRepository.GenerateMock<IDestination<string>>();
            mockDestination.Stub(destination => destination.ConsumeAsync(new[] { "one", "two", "three" }))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(items =>
                {
                    Console.WriteLine("--------------");
                    return Task.FromResult(items.Count());
                }));
            var helper = new ConsumerHelper<string>(mockDestination, 1);

            // Act
            var consumed = await helper.ConsumeAsync(new[] { "one", "two", "three" }, new CancellationToken());

            consumed.Should().Be(3);
            mockDestination.AssertWasCalled(
                destination =>
                    destination.ConsumeAsync(Arg<IEnumerable<string>>.Matches(actual => Compare(actual, new string[]{"one","two","three"}))));
        }

        [Test(Description = "One Consumers are greater than size, should use less consumers.")]
        public async Task ConsumesMoreThanItemsShouldUseLessConsumers()
        {
            var rand = new Random();
            var consumeData = new Dictionary<int, IEnumerable<string>>();
            // Arrange
            var mockDestination = MockRepository.GenerateMock<IDestination<string>>();
            mockDestination.Stub(destination => destination.ConsumeAsync(new[] { "one", "two" }))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(items =>
                {
                    consumeData[rand.Next()] = items.ToList();
                    Console.WriteLine("-------------");
                    foreach (var item in items)
                    {
                        Console.WriteLine(item);
                    }
                    Console.WriteLine("-------------");
                    return Task.FromResult(items.Count());
                }));
            var helper = new ConsumerHelper<string>(mockDestination, 3);

            // Act
            var consumed = await helper.ConsumeAsync(new[] { "one", "two"}, new CancellationToken());

            consumed.Should().Be(2);
            consumeData.Keys.Count.Should().Be(2);      // two consumers should exists
            consumeData.First().Value.Count().Should().Be(1);
            consumeData.Last().Value.Count().Should().Be(1);
        }

        [Test(Description = "One Consumers are equal to size each consumer should call with one item.")]
        public async Task ConsumesEqualToSizeShouldCallDestinationWithOneItemEach()
        {
            var rand = new Random();
            var consumeData = new Dictionary<int, IEnumerable<string>>();
            // Arrange
            var mockDestination = MockRepository.GenerateMock<IDestination<string>>();
            mockDestination.Stub(destination => destination.ConsumeAsync(new[] { "one", "two" , "three"}))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(items =>
                {
                    consumeData[rand.Next()] = items.ToList();
                    Console.WriteLine("-------------");
                    foreach (var item in items)
                    {
                        Console.WriteLine(item);
                    }
                    Console.WriteLine("-------------");
                    return Task.FromResult(items.Count());
                }));
            var helper = new ConsumerHelper<string>(mockDestination, 3);

            // Act
            var consumed = await helper.ConsumeAsync(new[] { "one", "two", "three" }, new CancellationToken());

            consumed.Should().Be(3);
            consumeData.Keys.Count.Should().Be(3);      // two consumers should exists
            var consumedItems = consumeData.Values.SelectMany(s => s);
            consumedItems.Should().BeEquivalentTo(new string[] {"one", "two","three" });
        }

        [Test(Description = "One Consumers are less than size, should all consumers with each size more than 1.")]
        public async Task ConsumesLessThanSizeShouldCallDestination()
        {
            var rand = new Random();
            var consumeData = new Dictionary<int, IEnumerable<string>>();
            // Arrange
            var mockDestination = MockRepository.GenerateMock<IDestination<string>>();
            mockDestination.Stub(destination => destination.ConsumeAsync(new[] { "one", "two", "three","four","five","six","seven","eight","nine","ten" }))
                .IgnoreArguments()
                .Do((ConsumerDelegate<string>)(items =>
                {
                    consumeData[rand.Next()] = items.ToList();
                    Console.WriteLine("-------------");
                    foreach (var item in items)
                    {
                        Console.WriteLine(item);
                    }
                    Console.WriteLine("-------------");
                    return Task.FromResult(items.Count());
                }));
            var helper = new ConsumerHelper<string>(mockDestination, 3);

            // Act
            var consumed = await helper.ConsumeAsync(new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten" }, new CancellationToken());

            consumed.Should().Be(10);
            consumeData.Keys.Count.Should().Be(3);      // two consumers should exists
            // todo: match
        }


        private bool Compare(IEnumerable<string> actual, IEnumerable<string> expected)
        {
            actual.Should().BeEquivalentTo(expected);
            return true;
        }
    }
}
