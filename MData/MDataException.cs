using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
	public abstract class MDataException : Exception
	{
		public MDataException()
			: base()
		{

		}

		public MDataException(string message)
			: base(message)
		{

		}

		public MDataException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
	}
}