using System;
using NUnit.Framework;

namespace CareFusion.Dispensing.Test
{
    [TestFixture(Category = "Unit")]
    public class GuardFixture
    {
        private enum Foo { Foo1, Foo2 }

        [Test]
        public void ArgumentNotNullTest()
        {
            string arg1 = null;

            Assert.Throws(typeof(ArgumentNullException), () =>
                Guard.ArgumentNotNull(arg1, "arg1"));
        }

        [Test]
        public void ArgumentNotNullOrEmptyStringTest()
        {
            string arg1 = string.Empty;

            Assert.Throws(typeof(ArgumentException), () =>
                Guard.ArgumentNotNullOrEmptyString(arg1, "arg1"));
        }

        [Test]
        public void EnumValueIsDefinedTest()
        {
            Foo foo3 = (Foo)3;
            Assert.Throws(typeof(ArgumentException), () =>
                Guard.EnumValueIsDefined(typeof(Foo), foo3, "foo3"));
        }
    }
}
