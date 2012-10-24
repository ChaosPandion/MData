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

        [TestClass]
        [ExcludeFromCodeCoverage]
        public sealed class Tests : TestsBase
        {
            private static readonly Expression _expression = Expression.Parameter(typeof(object));

            private static readonly Record _record =
                new Record(
                    new List<List<List<IField>>> { 
                            new List<List<IField>> { 
                                new List<IField> { 
                                    new Field("A", typeof(int), 1), 
                                    new Field("B", typeof(string), "X") } } });

            [TestMethod]
            public void Test_Constructors()
            {
                DoesThrow<ArgumentNullException>(() => new RecordDynamicMetaObject(null, _record));
                DoesThrow<ArgumentNullException>(() => new RecordDynamicMetaObject(_expression, null));
                new RecordDynamicMetaObject(_expression, _record);
            }

            [TestMethod]
            public void Test__Method_GetDynamicMemberNames()
            {
                var r = new RecordDynamicMetaObject(_expression, _record);
                Assert.IsNotNull(r.GetDynamicMemberNames());
                Assert.IsTrue(r.GetDynamicMemberNames().SequenceEqual(_record.GetFieldNames()));
            }

            [TestMethod]
            public void Test__Method_BindGetMember()
            {
                var r = new RecordDynamicMetaObject(_expression, _record);
                DoesThrow<ArgumentNullException>(() => r.BindGetMember(null));
            }
        }
    }
}