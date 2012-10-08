using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
	public sealed class NoRecordException : MDataException
	{
		public NoRecordException()
			: base("The result set contains no records.")
		{

		}
	}
}