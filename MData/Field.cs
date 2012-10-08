using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public sealed class Field
    {
        private readonly string _name;
        private readonly Type _type;
        private readonly object _value;

        internal Field(string name, Type type, object value)
        {
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
