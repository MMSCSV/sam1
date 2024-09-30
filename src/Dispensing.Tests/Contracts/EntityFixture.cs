using NUnit.Framework;

namespace CareFusion.Dispensing.Contracts.Test
{
    internal struct FooValueType
    {
        public int Item { get; set; }
    }
    internal class FooReferenceType { }
    internal class ValueTypeEntity : Entity<FooValueType> { }
    internal class NullableValueTypeEntity : Entity<FooValueType?> { }
    internal class PrimitiveEntity : Entity<long> { }
    internal class NullablePrimitiveEntity : Entity<int?> { }
    internal class CharPrimitiveEntity : Entity<char> { }
    internal class NullableCharPrimitiveEntity : Entity<char?> { }
    internal class ReferenceEntity : Entity<FooReferenceType> { }

    [TestFixture]
    public class EntityFixture
    {
        [Test]
        public void IsTransientValueTypeTest()
        {
            ValueTypeEntity entity = new ValueTypeEntity();
            Assert.IsTrue(entity.IsTransient());

            entity.Key = new FooValueType { Item = 1 };
            Assert.IsFalse(entity.IsTransient());
        }

        [Test]
        public void IsTransientNullableValueTypeTest()
        {
            NullableValueTypeEntity entity = new NullableValueTypeEntity();
            Assert.IsTrue(entity.IsTransient());

            entity.Key = new FooValueType { Item = 1 };
            Assert.IsFalse(entity.IsTransient());
        }

        [Test]
        public void IsTransientPrimitiveTypeTest()
        {
            PrimitiveEntity entity = new PrimitiveEntity();
            Assert.IsTrue(entity.IsTransient());

            entity.Key = -1;
            Assert.IsTrue(entity.IsTransient());

            entity.Key = 1;
            Assert.IsFalse(entity.IsTransient());
        }

        [Test]
        public void IsTransientNullablePrimitiveTypeTest()
        {
            NullablePrimitiveEntity entity = new NullablePrimitiveEntity();
            Assert.IsTrue(entity.IsTransient());

            entity.Key = 1;
            Assert.IsFalse(entity.IsTransient());
        }

        [Test]
        public void IsTransientCharPrimitiveTypeTest()
        {
            CharPrimitiveEntity entity = new CharPrimitiveEntity();
            Assert.IsTrue(entity.IsTransient());

            entity.Key = 'a';
            Assert.IsFalse(entity.IsTransient());
        }

        [Test]
        public void IsTransientNullableCharPrimitiveTypeTest()
        {
            NullableCharPrimitiveEntity entity = new NullableCharPrimitiveEntity();
            Assert.IsTrue(entity.IsTransient());

            entity.Key = 'a';
            Assert.IsFalse(entity.IsTransient());
        }

        [Test]
        public void IsTransientReferenceTypeTest()
        {
            ReferenceEntity entity = new ReferenceEntity();
            Assert.IsTrue(entity.IsTransient());

            entity.Key = new FooReferenceType();
            Assert.IsFalse(entity.IsTransient());
        }
    }
}
