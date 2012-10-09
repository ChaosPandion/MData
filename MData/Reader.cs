using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MData
{
	public sealed class Reader : IDisposable
	{
		private readonly IDataReader _reader;

		internal Reader(IDataReader reader)
		{
			_reader = reader;
		}

		public int FieldCount
		{
			get { return _reader.FieldCount; }
		}

		public bool ReadResult()
		{
			return _reader.NextResult();
		}

		public bool ReadRecord()
		{
			return _reader.Read();
		}

		public T ReadField<T>(string name)
		{
			return ReadField<T>(_reader.GetOrdinal(name));
		}

		public T ReadField<T>(int index)
		{
			if (_reader.IsDBNull(index))
				return default(T);
			return (T)_reader.GetValue(index);
		}

		public string GetFieldName(int index)
		{
			return _reader.GetName(index);
		}

		public IEnumerable<string> GetFieldNames()
		{
			for (int i = 0; i < _reader.FieldCount; i++)
				yield return _reader.GetName(i);
		}

		public Field ReadField(int index)
		{
			var name = _reader.GetName(index);
			var type = _reader.GetFieldType(index);
			var value = _reader.GetValue(index);
			if (_reader.IsDBNull(index))
			{
				value = null;
				if (type.IsValueType)
				{
					value = Activator.CreateInstance(typeof(Nullable<>).MakeGenericType(type));
				}
			}
			return new Field(name, type, value);
		}

		public void Dispose()
		{
			_reader.Dispose();
		}
	}
}