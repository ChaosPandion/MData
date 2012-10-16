using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MData.Foundation
{
	public class Results : ReadOnlyCollection<IResult>, IResultCollection
	{
		public Results(IList<IResult> records)
			: base(records)
		{

		}
	}
}
