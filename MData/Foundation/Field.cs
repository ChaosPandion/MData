using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
