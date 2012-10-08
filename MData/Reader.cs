using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
	public sealed class Reader : IDisposable
	{
		private readonly ADO ADO;

		internal Reader(ADO ado)
		{
			ADO = ado;
		}

		public int FieldCount
		{
			get { return ADO.Reader.FieldCount; }
		}

		public bool ReadResult()
		{
			return ADO.Reader.NextResult();
		}

		public bool ReadRecord()
		{
			return ADO.Reader.Read();
		}

		public T ReadField<T>(string name)
		{
			return ReadField<T>(ADO.Reader.GetOrdinal(name));
		}

		public T ReadField<T>(int index)
		{
			if (ADO.Reader.IsDBNull(index))
				return default(T);
			return (T)ADO.Reader.GetValue(index);
		}

		public string GetFieldName(int index)
		{
			return ADO.Reader.GetName(index);
		}

		public IEnumerable<string> GetFieldNames()
		{
			for (int i = 0; i < ADO.Reader.FieldCount; i++)
				yield return ADO.Reader.GetName(i);
		}

		public Field ReadField(int index)
		{
			var name = ADO.Reader.GetName(index);
			var type = ADO.Reader.GetFieldType(index);
			var value = ADO.Reader.GetValue(index);
			if (ADO.Reader.IsDBNull(index))
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
			ADO.Dispose();
		}
	}
}