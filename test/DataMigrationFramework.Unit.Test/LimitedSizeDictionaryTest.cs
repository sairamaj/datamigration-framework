using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace DataMigrationFramework.Unit.Test
{
    [TestFixture()]
    public class LimitedSizeDictionaryTest
    {
        [Test]
        public void AddMoreThanLimitShouldTrim()
        {
            // Arrange.
            var limitedSizeDictionary = new LimitedSizeDictionary<string, string>(
                10, 
                2,
                Comparer<string>.Create((x, y) => String.Compare(x, y, StringComparison.Ordinal)));

            // Act
            for (var i = 0; i < 15; i++)
            {
                limitedSizeDictionary.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }

            // Assert.
            limitedSizeDictionary.Dictionary.Count.Should().BeLessThan(12, $"Should have trimmed.");
        }

        [Test]
        public void AddAfterTrimShouldRemoveOldValues()
        {
            // Arrange.
            var limitedSizeDictionary = new LimitedSizeDictionary<string, string>(
                10, 
                2,
                Comparer<string>.Create((x, y) => String.Compare(x, y, StringComparison.Ordinal)));

            // Act
            for (var i = 0; i < 15; i++)
            {
                limitedSizeDictionary.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }

            // Assert.
            for (var i = 0; i < 4; i++)
            {
                limitedSizeDictionary.Dictionary.ContainsKey(i.ToString()).Should().BeFalse($"{i} should not exist.");
            }
        }

        [Test]
        public void AddingSameKeyShouldNotTrimAsItDoesNotChangeDictionary()
        {
            // Arrange.
            var limitedSizeDictionary = new LimitedSizeDictionary<string, string>(
                10, 
                2,
                Comparer<string>.Create((x, y) => String.Compare(x, y, StringComparison.Ordinal)));
            for (var i = 0; i < 10; i++)
            {
                limitedSizeDictionary.Add(new KeyValuePair<string, string>(i.ToString(), i.ToString()));
            }

            // Act
            for (int i = 0; i < 10; i++)
            {
                limitedSizeDictionary.Add(new KeyValuePair<string, string>(5.ToString(), i.ToString()));    // add same key.
            }

            // Assert.
            for (var i = 0; i < 4; i++)
            {
                limitedSizeDictionary.Dictionary.ContainsKey(i.ToString()).Should().BeTrue($"{i} should not exist.");
            }

        }
    }
}
