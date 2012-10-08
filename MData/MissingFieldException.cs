using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
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
}