using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MData.Foundation
{
	public class RecordSet : ReadOnlyCollection<IRecord>, IResult
	{
		public RecordSet(IList<IRecord> records)
			: base(records)
		{

		}
	}
}