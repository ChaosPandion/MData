using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
	public sealed class FieldMissingException : MDataException
	{
		public FieldMissingException(int index)
			: base(string.Format("No field exists with an index of {0}.", index))
		{

		}

		public FieldMissingException(string name)
			: base(string.Format("No field exists with the name '{0}'.", name))
		{

		}
	}
}