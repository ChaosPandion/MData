using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public sealed class RecordSet : IEnumerable<FieldSet>
    {
        private readonly List<FieldSet> _list = new List<FieldSet>();

		internal RecordSet(Reader r)
        {
			while (r.ReadRecord())
				_list.Add(new FieldSet(r));
        }

		public int Count
		{
			get { return _list.Count; }
		} 

        public IEnumerator<FieldSet> GetEnumerator()
        {
			return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
