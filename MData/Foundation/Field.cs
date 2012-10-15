using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MData.Support;

namespace MData.Foundation
{
    public sealed class Field : IField
    {
        private readonly string _name;
        private readonly Type _type;
        private readonly object _value;

        public Field(string name, Type type, object value)
        {
            name.ThrowIfNull("name");
            type.ThrowIfNull("type");
            if (value != null && value.GetType() != type)
                throw new Exception();
            _name = name;
            _type = type;
            _value = value;
        }

        public string Name
        {
            get { return _name; }
        }

        public Type Type
        {
            get { return _type; }
        }

        public object Value
        {
            get { return _value; }
        }

		public override bool Equals(object obj)
		{
			return Equals(obj as IField);
		}

		public bool Equals(IField other)
		{
			return other != null 
				&& this.Name == other.Name 
				&& this.Type == other.Type 
				&& object.Equals(this.Value, other.Value);
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 31 + _name.GetHashCode();
			hash = hash * 31 + _type.GetHashCode();
			hash = hash * 31 + (_value != null ? _value.GetHashCode() : 0);
			return hash;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", Name, Type);
		}
	}
}