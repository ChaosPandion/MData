using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace MData.Foundation
{
	public sealed class Reader : IReader
	{
        private readonly IDataReader _reader;

        public Reader(IDataReader reader)
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
        
        public IField GetField(int index)
        {
            return new Field(
                _reader.GetName(index), 
                _reader.GetFieldType(index), 
                GetFieldValue<object>(index));
        }

        public IField GetField(string name)
        {
            return new Field(name,
                _reader.GetFieldType(_reader.GetOrdinal(name)),
                GetFieldValue<object>(name));
        }

        public T GetFieldValue<T>(int index)
        {
            if (_reader.IsDBNull(index))
                return default(T);
            return (T)_reader.GetValue(index);
        }

        public T GetFieldValue<T>(string name)
        {
            return GetFieldValue<T>(GetFieldIndex(name));
        }

        public string GetFieldName(int index)
        {
            return _reader.GetName(index);
        }

        public int GetFieldIndex(string name)
        {
            return _reader.GetOrdinal(name);
        }

        public IEnumerable<string> GetFieldNames()
        {
            for (int i = 0; i < _reader.FieldCount; i++)
                yield return _reader.GetName(i);
        }

        public IEnumerable<IField> GetFields()
        {
            for (int i = 0; i < _reader.FieldCount; i++)
                yield return GetField(i);
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}