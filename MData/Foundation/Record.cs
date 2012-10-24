using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace MData.Foundation
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

        /// <summary>
		/// Creates a new immutable instance using the supplied fields.
        /// </summary>
        /// <param name="fields">The fields that will be contained in this record.</param>
        public Record(IEnumerable<IField> fields)
		{
			if (fields == null)
				throw new ArgumentNullException("fields");
            foreach (var field in fields)
			{
				if (field == null)
					throw new ArgumentException("Cannot contain null values.", "fields");
                _list.Add(field);
                _map.Add(field.Name, field);
            }
        }

		/// <summary>
		/// Gets the number of fields.
		/// </summary>
		public int Count
		{
			get { return _list.Count; }
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
			if (parameter == null)
				throw new ArgumentNullException("parameter");
			return new RecordDynamicMetaObject(parameter, this);
		}

		[DebuggerStepThrough]
		sealed class RecordDynamicMetaObject : DynamicMetaObject
		{
			private static readonly Type _recordType = typeof(Record);
			private static readonly MethodInfo _getFieldMethod = _recordType.GetMethod("GetField", new [] { typeof(string) });

			public RecordDynamicMetaObject(Expression expression, Record record)
				: base(expression, BindingRestrictions.Empty, record)
			{

			}

			public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
			{
				if (binder == null)
					throw new ArgumentNullException("binder");
				return new DynamicMetaObject(
					Expression.Call(
						Expression.Convert(
							Expression,
							_recordType),
						_getFieldMethod, 
						Expression.Constant(binder.Name)),
					BindingRestrictions.GetTypeRestriction(
						Expression, 
						LimitType), 
					Value);
			}
		}
    }
}