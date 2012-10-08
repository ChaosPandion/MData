using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace MData
{
    public sealed class FieldSet : DynamicObject, IEnumerable<Field>
    {
        private readonly Dictionary<string, Field> _map = new Dictionary<string, Field>();
        private readonly List<Field> _list = new List<Field>();

		internal FieldSet(Reader r)
        {
			for (int i = 0; i < r.FieldCount; i++)
			{
				var field = r.ReadField(i);
				_list.Add(field);
				_map.Add(field.Name, field);
			}
        }

        public int Count
        {
            get { return _list.Count; }
        }

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			Field field;
			if (!_map.TryGetValue(binder.Name, out field))
				throw new MissingFieldException(binder.Name);
			result = field.Value;
			return true;
		}

        public T Get<T>(int index)
        {
			if (index < 0 || index > _list.Count)
				throw new MissingFieldException(index);
            return (T)_list[index].Value;
        }

        public T Get<T>(string name)
		{
			Field field;
			if (!_map.TryGetValue(name ?? "", out field))
				throw new MissingFieldException(name);
			return (T)field.Value;
        }

        public IEnumerator<Field> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}