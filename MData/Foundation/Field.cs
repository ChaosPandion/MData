using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Dynamic;

namespace MData.Foundation
{
    /// <summary>
    /// Represents a named value.
    /// </summary>
    //[DebuggerStepThrough]
    public sealed class Field : DynamicObject, IField
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
            name.ThrowIfNull("name");
            type.ThrowIfNull("type");
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
        /// Provides implementation for type conversion operations. Classes derived from
        /// the System.Dynamic.DynamicObject class can override this method to specify
        /// dynamic behavior for operations that convert an object from one type to another.
        /// </summary>
        /// <param name="binder">
        /// Provides information about the conversion operation. The binder.
        /// Type property provides the type to which the object must be converted.
        /// </param>
        /// <param name="result">The result of the type conversion operation.</param>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            try
            {
                var type = Nullable.GetUnderlyingType(binder.Type) ?? binder.Type;
                result = Convert.ChangeType((IConvertible)Value, type);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
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
			return string.Format("{0}({1})", Name, Type);
		}
	}
}