using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.Support
{
    public struct Option<T>
    {
        private readonly T _value;
        private readonly bool _isSome;

        public Option(T value)
        {
            _value = value;
            _isSome = true;
        }

        public T Value
        {
            get
            {
                if (!_isSome)
                    throw new InvalidOperationException();
                return _value;
            }
        }

        public bool IsSome
        {
            get { return _isSome; }
        }

        public bool IsNone
        {
            get { return !_isSome; }
        }

        public override string ToString()
        {
            if (_isSome)
                return "Some";
            return "None";
        }
    }
}