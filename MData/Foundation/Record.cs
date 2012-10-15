using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Diagnostics;

namespace MData.Foundation
{
    public sealed class Record : DynamicObject, IRecord
    {
        private readonly Dictionary<string, IField> _map = new Dictionary<string, IField>();
        private readonly List<IField> _list = new List<IField>();

        public Record(IEnumerable<IField> fields)
        {
            Debug.Assert(fields != null);
            foreach (var field in fields)
            {
                Debug.Assert(field != null);
                Debug.Assert(field.Name != null);

                _list.Add(field);
                _map.Add(field.Name, field);
            }
        }

        public T GetValue<T>(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new Exception();
			return (T)_list[index].Value;
        }

        public T GetValue<T>(string name)
        {
			IField field;
			if (!_map.TryGetValue(name, out field))
				throw new Exception();
			return (T)field.Value;
		}

		public Option<T> TryGetValue<T>(int index)
		{
			if (index < 0 || index >= _list.Count)
				return Option.None<T>();
			return Option.Some((T)_list[index].Value);
		}

		public Option<T> TryGetValue<T>(string name)
		{
			IField field;
			if (!_map.TryGetValue(name, out field))
				return Option.None<T>();
			return Option.Some((T)field.Value);
		}

		public IField GetField(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new Exception();
			return _list[index];
		}

		public IField GetField(string name)
		{
			IField field;
			if (!_map.TryGetValue(name, out field))
				throw new Exception();
			return field;
		}

		public Option<IField> TryGetField(int index)
		{
			if (index < 0 || index >= _list.Count)
				return Option.None<IField>();
			return Option.Some(_list[index]);
		}

		public Option<IField> TryGetField(string name)
		{
			IField field;
			if (!_map.TryGetValue(name, out field))
				return Option.None<IField>();
			return Option.Some(field);
		}

        public IEnumerator<IField> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			IField field;
			if (!_map.TryGetValue(binder.Name, out field))
				throw new Exception();
			result = field;
			return true;
		}
	}
}