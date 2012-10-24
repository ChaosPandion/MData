using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MData
{
    [DebuggerStepThrough]
    public sealed class RecordDynamicMetaObject : DynamicMetaObject
    {
        private static readonly Type _recordType = typeof(Record);
        private static readonly MethodInfo _getFieldMethod = _recordType.GetMethod("GetField", new[] { typeof(string) });

        public RecordDynamicMetaObject(Expression expression, Record record)
            : base(expression, BindingRestrictions.Empty, record)
        {
            if (record == null)
                throw new ArgumentNullException("record");
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

        [TestClass]
        [ExcludeFromCodeCoverage]
        public sealed class Tests
        {
            //private static readonly Expression _parameter = Expression.Parameter(typeof(object));
            //private static readonly Record _record = new Record(new[] { new Field("A", typeof(int), 1) });

            //[TestMethod]
            //[ExpectedException(typeof(ArgumentNullException))]
            //public void ThrowsWhenConstructedWithNullExpression()
            //{
            //    new RecordDynamicMetaObject(null, _record);
            //}

            //[TestMethod]
            //[ExpectedException(typeof(ArgumentNullException))]
            //public void ThrowsWhenConstructedWithNullField()
            //{
            //    new RecordDynamicMetaObject(_parameter, null);
            //}

            //[TestMethod]
            //[ExpectedException(typeof(ArgumentNullException))]
            //public void BindGetMemberThrowsWhenConstructedWithNullField()
            //{
            //    new RecordDynamicMetaObject(_parameter, _record).BindGetMember(null);
            //}
        }
    }
}
