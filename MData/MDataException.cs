using System;
using System.Runtime.Serialization;

namespace MData
{
	public class MDataException : Exception
	{
		public MDataException()
			: base()
		{

		}

		public MDataException(string message)
			: base(message)
		{

		}

		protected MDataException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{

		}

		public MDataException(string message, Exception innerException)
			: base(message, innerException)
		{

		}
	}
}