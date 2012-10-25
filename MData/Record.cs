using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MData
{
    /// <summary>
	/// Represents a collection of fields.
    /// </summary>
	[DebuggerStepThrough]
    public sealed class Record : IRecord
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<string, IField> _map = new Dictionary<string, IField>();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly List<IField> _list = new List<IField>();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _resultIndex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int _recordIndex;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IRecord _nextResult;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IRecord _nextRecord;

        /// <summary>
        /// Constructs a new immutable instance that contains the provided results.
        /// </summary>
        /// <param name="results">The results that will be included.</param>
        public Record(IEnumerable<IEnumerable<IEnumerable<IField>>> results)
        {
            if (results == null)
                throw new ArgumentNullException("results");
            var resultIndex = 0;
            var r = this;
            foreach (var result in results)
            {
                if (result == null)
                    throw new ArgumentException("Cannot contain a null result.", "results");
                if (resultIndex > 0)
                {
                    r._nextResult = new Record();
                    r = (Record)r._nextResult;
                }
                var recordIndex = 0;
                foreach (var record in result)
                {
                    if (record == null)
                        throw new ArgumentException("Cannot contain a null record.", "results");
                    if (recordIndex > 0)
                    {
                        r._nextRecord = new Record();
                        r = (Record)r._nextRecord;
                    }
                    foreach (var field in record)
                    {
                        if (field == null)
                            throw new ArgumentException("Cannot contain a null field.", "results");
                        r._list.Add(field);
                        r._map.Add(field.Name, field);
                    }
                    r._recordIndex = recordIndex;
                    recordIndex++;
                }
                r._resultIndex = resultIndex;
                resultIndex++;
            }
        }

        private Record()
        {

        }

        /// <summary>
        /// Gets the number of results within the result set.
        /// </summary>
        public int ResultCount
        {
            get 
            {
                var x = this;
                do
                {
                    if (x._nextResult != null)
                    {
                        x = (Record)x._nextResult;
                    }
                    else if (x.NextRecord != null)
                    {
                        x = (Record)x._nextRecord;
                    }
                    else
                    {
                        return x._resultIndex + 1;
                    }
                } while (true);
            }
        }

        /// <summary>
        /// Gets the number of records within the record set.
        /// </summary>
        public int RecordCount
        {
            get
            {
                var x = this;
                do
                {
                    if (x._nextRecord != null)
                    {
                        x = (Record)x._nextRecord;
                    }
                    else
                    {
                        return x._recordIndex + 1;
                    }
                } while (true);
            }
        }

        /// <summary>
        /// Gets the index of the result within the result set.
        /// </summary>
        public int ResultIndex
        {
            get { return _resultIndex; }
        }

        /// <summary>
        /// Gets the index of the record within the record set.
        /// </summary>
        public int RecordIndex
        {
            get { return _recordIndex; }
        }

        /// <summary>
        /// Gets the next result in the result set or null if there are no more results.
        /// </summary>
        public IRecord NextResult
        {
            get 
            { 
                if (_nextResult != null)
                    return _nextResult;
                var x = this;
                while (x._nextRecord != null)
                    x = (Record)x._nextRecord;
                return x._nextResult;
            }
        }

        /// <summary>
        /// Gets the next record in the record set or null if there are no more records.
        /// </summary>
        public IRecord NextRecord
        {
            get { return _nextRecord; }
        }

		/// <summary>
		/// Gets the number of fields.
		/// </summary>
		public int FieldCount
		{
			get { return _list.Count; }
		}

        /// <summary>
        /// Gets a collection containing the field names.
        /// </summary>
        public IEnumerable<string> GetFieldNames()
        {
            return _list.Select(f => f.Name);
        }

        /// <summary>
        /// Returns the name of the field with the specified <paramref name="index" />. 
        /// </summary>
        /// <param name="index">The index of the field.</param>
        public string GetName(int index)
        {
            return GetField(index).Name;
        }

        /// <summary>
        /// Returns the type of the field with the specified <paramref name="index" />. 
        /// </summary>
        /// <param name="index">The index of the field.</param>
        public Type GetType(int index)
        {
            return GetField(index).Type;
        }

        /// <summary>
        /// Returns the type of the field with the specified name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        public Type GetType(string name)
        {
            return GetField(name).Type;
        }

		/// <summary>
		/// Gets the value of the field at the specified index.
		/// </summary>
		/// <typeparam name="T">The expected type of the field.</typeparam>
		/// <param name="index">The index of the field.</param>
        public T GetValue<T>(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new FieldMissingException(index);
			return (T)_list[index].Value;
        }

		/// <summary>
		/// Gets the value of the field with the specified name.
		/// </summary>
		/// <typeparam name="T">The expected type of the field.</typeparam>
		/// <param name="name">The name of the field.</param>
        public T GetValue<T>(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			IField field;
			if (!_map.TryGetValue(name, out field))
				throw new FieldMissingException(name);
			return (T)field.Value;
		}

		/// <summary>
		/// Gets the field at the specified index.
		/// </summary>
		/// <param name="index">The index of the field.</param>
		public IField GetField(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new FieldMissingException(index);
			return _list[index];
		}

		/// <summary>
		/// Gets the field with the specified name.
		/// </summary>
		/// <param name="name">The name of the field.</param>
		public IField GetField(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			IField field;
			if (!_map.TryGetValue(name, out field))
				throw new FieldMissingException(name);
			return field;
		}

        /// <summary>
        /// Returns an enumerator that iterates through this result.
        /// Iteration will start from the current record rather than 
        /// the first record.
        /// </summary>
        public IEnumerable<IRecord> EnumerateRecords()
        {
            var r = (IRecord)this;
            do
            {
                yield return r;
                r = r.NextRecord;
            } while (r != null);
        }

        /// <summary>
        /// Returns an enumerator that iterates through this record.
        /// </summary>
        public IEnumerator<IField> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
		{
			return new RecordDynamicMetaObject(parameter, this);
		}

        [DebuggerStepThrough]
        internal sealed class RecordDynamicMetaObject : DynamicMetaObject
        {
            public RecordDynamicMetaObject(Expression expression, Record record)
                : base(expression, BindingRestrictions.Empty, record)
            {
                if (record == null)
                    throw new ArgumentNullException("record");
            }

            public override IEnumerable<string> GetDynamicMemberNames()
            {
                return ((IRecord)Value).Select(f => f.Name);
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                if (binder == null)
                    throw new ArgumentNullException("binder");
                var val = Expression.Convert(Expression, typeof(IFieldMap));
                var getValue = Expression.Call(val, "GetValue", new[] { typeof(object) }, Expression.Constant(binder.Name));
                return new DynamicMetaObject(getValue, BindingRestrictions.GetTypeRestriction(Expression, LimitType), Value);
            }
        }
    }
}