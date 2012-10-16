using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MData.Foundation
{
	public class Result : ReadOnlyCollection<IRecord>, IResult
	{
		public Result(IList<IRecord> records)
			: base(records)
		{

		}
	}
}