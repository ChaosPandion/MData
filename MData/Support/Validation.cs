using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData.Support
{
    static class Validation
    {
        public static void ThrowIfNull(this object value, string name)
        {
            if (object.ReferenceEquals(value, null))
                throw new ArgumentNullException(name);
        }

        public static void ThrowIfNullOrWhiteSpace(this string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("This value cannot be null, empty, or only white space.", name);
        }
    }
}
