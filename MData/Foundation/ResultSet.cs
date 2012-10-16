using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MData.Foundation
{
	public class ResultSet : ReadOnlyCollection<IRecordSet>, IResultSet
	{
		public ResultSet(IList<IRecordSet> records)
			: base(records)
		{

		}
	}
}
