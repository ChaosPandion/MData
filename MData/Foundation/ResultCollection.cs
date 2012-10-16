using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MData.Foundation
{
	public class ResultCollection : ReadOnlyCollection<IResult>, IResultCollection
	{
		public ResultCollection(IList<IResult> records)
			: base(records)
		{

		}
	}
}
