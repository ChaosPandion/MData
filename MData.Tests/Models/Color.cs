using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData.Tests.Models
{
    public struct Color : IEquatable<Color>
    {
        private readonly byte _r;
        private readonly byte _g;
        private readonly byte _b;
        private readonly byte _a;

        public static readonly Color Red = new Color(255, 0, 0, 0);
        public static readonly Color Green = new Color(0, 255, 0, 0);
        public static readonly Color Blue = new Color(0, 0, 255, 0);
        public static readonly Color Yellow = new Color(255, 255, 0, 0);

        public Color(byte r, byte g, byte b, byte a)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public int R
        {
            get { return _r; }
        }

        public int G
        {
            get { return _g; }
        }

        public int B
        {
            get { return _b; }
        }

        public int A
        {
            get { return _a; }
        }

        public override bool Equals(object obj)
        {
            return obj is Color && base.Equals((Color)obj);
        }

        public bool Equals(Color other)
        {
            return this._r == other._r
                && this._g == other._g
                && this._b == other._b
                && this._a == other._a;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + _r;
            hash = hash * 31 + _g;
            hash = hash * 31 + _b;
            hash = hash * 31 + _a;
            return hash;
        }

        public override string ToString()
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", _r, _g, _b, _a);
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }
    }
}