using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public sealed class ResultSet : IEnumerable<RecordSet>
    {
        private readonly List<RecordSet> _values = new List<RecordSet>();

        internal ResultSet(Reader r)
        {
			do
			{
				_values.Add(new RecordSet(r));
			} while (r.ReadResult());
        }

		public int Count
		{
			get { return _values.Count; }
		}

        public IEnumerator<RecordSet> GetEnumerator()
        {
			return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}