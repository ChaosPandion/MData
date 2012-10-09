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

	public sealed class MissingFieldException : MDataException
	{
		public MissingFieldException(int index)
			: base(string.Format("No field exists at index {0}.", index))
		{

		}

		public MissingFieldException(string name)
			: base(string.Format("No field exists with the name '{0}'.", name ?? ""))
		{

		}
	}

	public sealed class NoRecordException : MDataException
	{
		public NoRecordException()
			: base("The result set contains no records.")
		{

		}
	}
}