using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq.Expressions;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

namespace MData
{
    /// <summary>
    /// Represents a named value.
    /// </summary>
    [DebuggerStepThrough]
    public sealed class Field : IField
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly string _name;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Type _type;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _value;

        /// <summary>
        /// Creates a new immutable instance using the supplied values.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="type">The type of the field.</param>
        /// <param name="value">The value of the field.</param>
        public Field(string name, Type type, object value)
        {
			if (name == null)
				throw new ArgumentNullException("name");
			if (type == null)
				throw new ArgumentNullException("type");
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                throw new ArgumentException("The value cannot be an instance of Nullable<T>.", "type");
            if (value != null && value.GetType() != type)
                throw new ArgumentException("The argument 'type' does not match the type of 'value'.");
            _name = name;
            _type = type;
            _value = value;
        }

        /// <summary>
        /// Gets the Name of the current instance.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the Type of the current instance.
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets the Value of the current instance.
        /// </summary>
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Determines whether the current <see cref="MData.Foundation.Field"/> is equal to the supplied <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
		public override bool Equals(object obj)
		{
			return Equals(obj as IField);
		}

        /// <summary>
        /// Determines whether the current <see cref="MData.Foundation.Field"/> is equal to the supplied <see cref="MData.IField"/>.
        /// </summary>
        /// <param name="other">The field to compare.</param>
		public bool Equals(IField other)
		{
			return other != null 
				&& this.Name == other.Name 
				&& this.Type == other.Type 
				&& object.Equals(this.Value, other.Value);
		}

        /// <summary>
        /// Returns the hash code for this field.
        /// </summary>
		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + _name.GetHashCode();
			hash = hash * 31 + _type.GetHashCode();
			hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);
			return hash;
		}

        /// <summary>
        /// Returns a string that represents this field.
        /// </summary>
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}({1})", Name, Type);
		}

		DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
		{
			return new FieldDynamicMetaObject(parameter, this);
		}


        [TestClass]
        [ExcludeFromCodeCoverage]
        public sealed class Tests
        {
            [TestMethod]
            [Description("XXXXXX")]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullName() { new Field(null, typeof(int), 1); }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullType() { new Field("A", null, 1); }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowWhenTypeDoesNotMatchValue() { new Field("A", typeof(string), 1); }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowWhenTypeIsNullableStruct() { new Field("A", typeof(int?), 1); }

            [TestMethod]
            public void ShouldNotThrowWhenValueIsNullableAndTypeIsUnderlyingType() { new Field("A", typeof(int), (int?)1); }

            [TestMethod]
            public void ShouldNotThrowWhenValueIsNullAndTypeIsNotNullable() { new Field("A", typeof(int), null); }

            [TestMethod]
            public void NameShouldNotBeNullAfterConstruction() { Assert.IsNotNull(new Field("A", typeof(int), 1).Name); }

            [TestMethod]
            public void TypeShouldNotBeNullAfterConstruction() { Assert.IsNotNull(new Field("A", typeof(int), 1).Type); }

            [TestMethod]
            public void ValueShouldNotBeNullAfterConstructionWhenPassedANonNullValue() { Assert.IsNotNull(new Field("A", typeof(int), 1).Value); }

            [TestMethod]
            public void ValueShouldBeNullAfterConstructionWhenPassedANullValue() { Assert.IsNull(new Field("A", typeof(int), null).Value); }

            [TestMethod]
            public void NameShouldMatchAfterConstruction() { Assert.AreEqual(new Field("A", typeof(int), 1).Name, "A"); }

            [TestMethod]
            public void TypeShouldMatchAfterConstruction() { Assert.AreEqual(new Field("A", typeof(int), 1).Type, typeof(int)); }

            [TestMethod]
            public void ValueShouldMatchAfterConstruction() { Assert.AreEqual(new Field("A", typeof(int), 1).Value, 1); }

            [TestMethod]
            public void ValueShouldDynamicallyConvertToType() { int v = (dynamic)new Field("A", typeof(int), 1); }

            [TestMethod]
            [ExpectedException(typeof(InvalidCastException))]
            public void ShouldThrowWhenNullValueDynamicallyConvertedToNonNullableValueType() { int v = (dynamic)new Field("A", typeof(int), null); }

            [TestMethod]
            public void ValueShouldDynamicallyConvertToNullableType() { int? v = (dynamic)new Field("A", typeof(int), 1); }

            [TestMethod]
            public void PrimitiveValueShouldDynamicallyConvertToOtherPrimitiveTypes() { decimal v = (dynamic)new Field("A", typeof(int), 1); }

            [TestMethod]
            public void PrimitiveValueShouldDynamicallyConvertToOtherPrimitiveNullableTypes() { decimal? v = (dynamic)new Field("A", typeof(int), 1); }

            [TestMethod]
            public void DistinctFieldObjectsWithIdenticalValuesShouldBeEqual() { Assert.AreEqual(new Field("A", typeof(int), 1), new Field("A", typeof(int), 1)); }

            [TestMethod]
            public void ToStringIsTheSameForEqualFields()
            {
                var f1 = new Field("A", typeof(int), 1);
                var f2 = new Field("A", typeof(int), 1);
                Assert.AreEqual(f1.ToString(), f2.ToString());
            }

            [TestMethod]
            public void GetHashCodeIsTheSameForEqualFields()
            {
                var f1 = new Field("A", typeof(int), 1);
                var f2 = new Field("A", typeof(int), 1);
                Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void GetMetaObjectThrowsWhenPassedNull() { ((IDynamicMetaObjectProvider)new Field("A", typeof(int), 1)).GetMetaObject(null); }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void BindConvertThrowsWhenPassedNull()
            {
                var f = new Field("A", typeof(int), 1);
                var p = (IDynamicMetaObjectProvider)f;
                var m = p.GetMetaObject(Expression.Parameter(typeof(object)));
                var r = m.BindConvert(null);
            }

            [TestMethod]
            public void BindConvertReturnsCorrectType()
            {
                var f = new Field("A", typeof(int), 1);
                var p = (IDynamicMetaObjectProvider)f;
                var param = Expression.Parameter(typeof(object));
                var m = p.GetMetaObject(param);
                var func = Expression.Lambda<Func<object, IField>>(Expression.Convert(m.Expression, typeof(IField)), param).Compile();
                Assert.AreEqual(f, func(f));
            }
        }
	}
}