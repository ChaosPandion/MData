using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MData.Support
{
    static class Validation
    {
		[DebuggerStepThrough]
        public static void ThrowIfNull(this object value, string name)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(name));
            if (object.ReferenceEquals(value, null))
                throw new ArgumentNullException(name);
        }

		[DebuggerStepThrough]
		public static void ThrowIfNullOrEmpty(this string value, string name)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(name));
			if (string.IsNullOrEmpty(value))
				throw new ArgumentException("This value cannot be null or empty.", name);
		}

		[DebuggerStepThrough]
        public static void ThrowIfNullOrWhiteSpace(this string value, string name)
		{
			Debug.Assert(!string.IsNullOrWhiteSpace(name));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("This value cannot be null, empty, or only white space.", name);
        }
    }
}
