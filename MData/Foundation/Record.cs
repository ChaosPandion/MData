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
    public sealed class Record : IRecord
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
            return (T)_list[index].Value;
        }

        public T GetValue<T>(string name)
        {
            return (T)_map[name].Value;
        }

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

        private sealed class RecordDynamicMetaObject : DynamicMetaObject
        {
            public RecordDynamicMetaObject(Expression expression, Record value)
                : base(expression, BindingRestrictions.Empty, value)
            {

            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                var instance = Expression.Convert(Expression, LimitType);
                var fieldName = Expression.Constant(binder.Name);
                return new DynamicMetaObject(
                    Expression.Call(instance, "Get", new[] { typeof(object) }, fieldName),
                    BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            }

        }

    }
}