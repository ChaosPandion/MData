using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData.Exceptions
{
    [Serializable]
	public abstract class MException : Exception
	{
		public MException()
			: base()
		{

		}

		public MException(string message)
			: base(message)
		{

		}

		public MException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
	}
}