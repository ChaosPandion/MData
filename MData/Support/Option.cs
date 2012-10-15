using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.Support
{
	public static class Option
	{
		public static Option<T> Some<T>(T value)
		{
			return new Option<T>(value);
		}

		public static Option<T> None<T>()
		{
			return new Option<T>();
		}
	}

	public struct Option<T> : IEquatable<T>
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

		public override bool Equals(object obj)
		{
			return obj is T && Equals((T)obj);
		}

		public bool Equals(T other)
		{
			return EqualityComparer<T>.Default.Equals(_value, other);
		}

		public override int GetHashCode()
		{
			return EqualityComparer<T>.Default.GetHashCode(_value);
		}

        public override string ToString()
        {
            if (_isSome)
                return "Some";
            return "None";
        }

		public static bool operator ==(Option<T> left, Option<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Option<T> left, Option<T> right)
		{
			return !left.Equals(right);
		}
	}
}