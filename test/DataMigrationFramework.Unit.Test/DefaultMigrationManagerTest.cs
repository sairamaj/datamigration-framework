using System;
using FluentAssertions;
using NUnit.Framework;

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
    }
}
