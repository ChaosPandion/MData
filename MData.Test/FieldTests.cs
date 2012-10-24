using System;
using MData.Foundation;
using NUnit.Framework;

namespace MData.Test
{
    [TestFixture]
    class FieldTests
    {
        [Test]
        public void ShouldThrowOnNullName() { Assert.Throws<ArgumentNullException>(() => new Field(null, typeof(int), 1)); }

        [Test]
        public void ShouldThrowOnNullType() { Assert.Throws<ArgumentNullException>(() => new Field("A", null, 1)); }

        [Test]
        public void ShouldThrowWhenTypeDoesNotMatchValue() { Assert.Throws<ArgumentException>(() => new Field("A", typeof(string), 1)); }

        [Test]
        public void ShouldThrowWhenTypeIsNullableStruct() { Assert.Throws<ArgumentException>(() => new Field("A", typeof(int?), 1)); }

        [Test]
        public void ShouldNotThrowWhenValueIsNullableAndTypeIsUnderlyingType() { new Field("A", typeof(int), (int?)1); }

        [Test]
        public void ShouldNotThrowWhenValueIsNullAndTypeIsNotNullable() { new Field("A", typeof(int), null); }

        [Test]
        public void NameShouldNotBeNullAfterConstruction() { Assert.IsNotNull(new Field("A", typeof(int), 1).Name); }

        [Test]
        public void TypeShouldNotBeNullAfterConstruction() { Assert.IsNotNull(new Field("A", typeof(int), 1).Type); }

        [Test]
        public void ValueShouldNotBeNullAfterConstructionWhenPassedANonNullValue() { Assert.IsNotNull(new Field("A", typeof(int), 1).Value); }

        [Test]
        public void ValueShouldBeNullAfterConstructionWhenPassedANullValue() { Assert.IsNull(new Field("A", typeof(int), null).Value); }

        [Test]
        public void NameShouldMatchAfterConstruction() { Assert.AreEqual(new Field("A", typeof(int), 1).Name, "A"); }

        [Test]
        public void TypeShouldMatchAfterConstruction() { Assert.AreEqual(new Field("A", typeof(int), 1).Type, typeof(int)); }

        [Test]
        public void ValueShouldMatchAfterConstruction() { Assert.AreEqual(new Field("A", typeof(int), 1).Value, 1); }

        [Test]
        public void ValueShouldDynamicallyConvertToType() { int v = (dynamic)new Field("A", typeof(int), 1); }

        [Test]
        public void ShouldThrowWhenNullValueDynamicallyConvertedToNonNullableValueType() { Assert.Throws<InvalidCastException>(() => { int v = (dynamic)new Field("A", typeof(int), null); }); }

        [Test]
        public void ValueShouldDynamicallyConvertToNullableType() { int? v = (dynamic)new Field("A", typeof(int), 1); }

        [Test]
        public void PrimitiveValueShouldDynamicallyConvertToOtherPrimitiveTypes() { decimal v = (dynamic)new Field("A", typeof(int), 1); }

        [Test]
        public void PrimitiveValueShouldDynamicallyConvertToOtherPrimitiveNullableTypes() { decimal? v = (dynamic)new Field("A", typeof(int), 1); }

        [Test]
        public void DistinctFieldObjectsWithIdenticalValuesShouldBeEqual() { Assert.AreEqual(new Field("A", typeof(int), 1), new Field("A", typeof(int), 1)); }
    }
}