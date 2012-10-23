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
    /// <summary>
    /// 
    /// </summary>
    public sealed class Record : IRecord
    {
        private readonly Dictionary<string, IField> _map = new Dictionary<string, IField>();
        private readonly List<IField> _list = new List<IField>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fields"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        public T GetValue<T>(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new Exception();
			return (T)_list[index].Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public T GetValue<T>(string name)
        {
			IField field;
			if (!_map.TryGetValue(name, out field))
				throw new Exception();
			return (T)field.Value;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
		public IField GetField(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new Exception();
			return _list[index];
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
		public IField GetField(string name)
		{
			IField field;
			if (!_map.TryGetValue(name, out field))
				throw new Exception();
			return field;
		}

        /// <summary>
        /// 
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
            throw new NotImplementedException();
        }

        class RecordDynamicMetaObject : DynamicMetaObject
        {
            public RecordDynamicMetaObject(Expression expression, Record record)
                : base(expression, BindingRestrictions.Empty, record) 
            {

            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                return base.BindGetMember(binder);
            }
        }

        //public override bool TryGetMember(GetMemberBinder binder, out object result)
        //{
        //    IField field;
        //    if (!_map.TryGetValue(binder.Name, out field))
        //        throw new Exception();
        //    result = field;
        //    return true;
        //}
    }
}