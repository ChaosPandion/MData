using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq.Expressions;
using System.Globalization;

namespace MData.Foundation
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

		[DebuggerStepThrough]
		sealed class FieldDynamicMetaObject : DynamicMetaObject
		{
			public FieldDynamicMetaObject(Expression expression, Field field)
				: base(expression, BindingRestrictions.Empty, field)
			{
				if (field == null)
					throw new ArgumentNullException("field");
			}

			public override DynamicMetaObject BindConvert(ConvertBinder binder)
			{
				if (binder == null)
					throw new ArgumentNullException("binder");
				return new DynamicMetaObject(
					Expression.Convert(
						Expression.Constant(
							Convert.ChangeType(
								((Field)Value).Value, 
								Nullable.GetUnderlyingType(binder.Type) ?? binder.Type,
								CultureInfo.InvariantCulture)), 
						binder.Type),
					BindingRestrictions.GetTypeRestriction(
						Expression, 
						LimitType));
			}
		}
	}
}