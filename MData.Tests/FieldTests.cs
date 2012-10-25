using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace MData.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public sealed class FieldTests
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowOnNullName() { new Field(null, typeof(int), 1); }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowOnNullType() { new Field("A", null, 1); }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenTypeDoesNotMatchValue() { new Field("A", typeof(string), 1); }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowWhenTypeIsNullableStruct() { new Field("A", typeof(int?), 1); }

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
        public void DistinctFieldObjectsWithIdenticalValuesShouldBeEqual() { Assert.AreEqual(new Field("A", typeof(int), 1), new Field("A", typeof(int), 1)); }

        [Test]
        public void ToStringIsTheSameForEqualFields()
        {
            var f1 = new Field("A", typeof(int), 1);
            var f2 = new Field("A", typeof(int), 1);
            Assert.AreEqual(f1.ToString(), f2.ToString());
        }

        [Test]
        public void GetHashCodeIsTheSameForEqualFields()
        {
            var f1 = new Field("A", typeof(int), 1);
            var f2 = new Field("A", typeof(int), 1);
            Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
        }
    }
}