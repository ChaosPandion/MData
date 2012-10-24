using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Globalization;
using System.Linq.Expressions;

namespace MData
{
    [DebuggerStepThrough]
    public sealed class FieldDynamicMetaObject : DynamicMetaObject
    {
        public FieldDynamicMetaObject(Expression expression, Field field)
            : base(expression, BindingRestrictions.Empty, field)
        {
            if (field == null)
                throw new ArgumentNullException("field");
        }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            if (binder == null)
                throw new ArgumentNullException("binder");
            return new DynamicMetaObject(
                Expression.Convert(
                    Expression.Constant(
                        Convert.ChangeType(
                            ((Field)Value).Value,
                            Nullable.GetUnderlyingType(binder.Type) ?? binder.Type,
                            CultureInfo.InvariantCulture)),
                    binder.Type),
                BindingRestrictions.GetTypeRestriction(
                    Expression,
                    LimitType));
        }

        [TestClass]
        [ExcludeFromCodeCoverage]
        public sealed class Tests
        {
            private static readonly Expression _parameter = Expression.Parameter(typeof(object));
            private static readonly Field _field = new Field("A", typeof(int), 1);

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ThrowsWhenConstructedWithNullExpression()
            {
                new FieldDynamicMetaObject(null, _field);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ThrowsWhenConstructedWithNullField()
            {
                new FieldDynamicMetaObject(_parameter, null);
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void BindConvertThrowsWhenConstructedWithNullField()
            {
                new FieldDynamicMetaObject(_parameter, _field).BindConvert(null);
            }
        }
    }
}
