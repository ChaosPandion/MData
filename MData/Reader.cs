using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;

namespace MData
{
	public sealed class Reader : IReader
	{
        const int _startingIndex = -1;
        private readonly IDataReader _reader;
        private int _recordIndex = _startingIndex;

        public Reader(IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (reader.IsClosed)
                throw new ArgumentNullException("Reader cannot be closed.", "reader");
            _reader = reader;
        }

        public int ResultIndex
        {
            get { return _reader.Depth; }
        }

        public int RecordIndex
        {
            get { return _recordIndex; }
        }

        public int FieldCount
        {
            get { return _reader.FieldCount; }
        }

        public bool ReadResult()
        {
            var r = _reader.NextResult();
            if (r) _recordIndex = _startingIndex; 
            return r;
        }

        public bool ReadRecord()
        {
            var r = _reader.Read();
            if (r) _recordIndex++;
            return r;
        }

        public IEnumerable<string> GetFieldNames()
        {
            for (var i = 0; i < FieldCount; i++)
                yield return _reader.GetName(i);
        }

        public string GetName(int index)
        {
            return _reader.GetName(index);
        }

        public Type GetType(int index)
        {
            return _reader.GetFieldType(index);
        }

        public Type GetType(string name)
        {
            return _reader.GetFieldType(_reader.GetOrdinal(name));
        }

        public T GetValue<T>(int index)
        {
            if (_reader.IsDBNull(index))
                return default(T);
            return (T)_reader.GetValue(index);
        }

        public T GetValue<T>(string name)
        {
            var index = _reader.GetOrdinal(name);
            if (_reader.IsDBNull(index))
                return default(T);
            return (T)_reader.GetValue(index);
        }

        public IField GetField(int index)
        {
            return new Field(_reader.GetName(index), _reader.GetFieldType(index), GetValue<object>(index));
        }

        public IField GetField(string name)
        {
            var index = _reader.GetOrdinal(name);
            return new Field(_reader.GetName(index), _reader.GetFieldType(index), GetValue<object>(index));
        }

        public void Dispose()
        {
            _reader.Dispose();
        }

        public IEnumerator<IField> GetEnumerator()
        {
            for (int i = 0; i < FieldCount; i++)
                yield return GetField(i);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}