using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MData.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public sealed class RecordTests
    {
        private static readonly Record _1RES_1REC_2FLD =
                new Record(
                    new List<List<List<IField>>> { 
                            new List<List<IField>> { 
                                new List<IField> { 
                                    new Field("A", typeof(int), 1), 
                                    new Field("B", typeof(string), "X") } } });

        private static readonly Record _1RES_2REC_2FLD =
                new Record(
                    new List<List<List<IField>>> { 
                            new List<List<IField>> { 
                                new List<IField> { 
                                    new Field("A", typeof(int), 1), 
                                    new Field("B", typeof(string), "X") },
                                new List<IField> { 
                                    new Field("A", typeof(int), 1), 
                                    new Field("B", typeof(string), "X") } } });


        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowOnNullResults()
        {
            new Record(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnNullRecords()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>>(), null });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnNullRecord()
        {
            new Record(new List<List<List<IField>>>
                {
                    new List<List<IField>>
                    {
                        new List<IField>
                        {
                            new Field("A", typeof(int), 1),
                            new Field("B", typeof(int), 1),
                            new Field("C", typeof(int), 1)
                        },                
                        null
                    }
                });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldThrowOnNullField()
        {
            new Record(new List<List<List<IField>>>
                {
                    new List<List<IField>>
                    {
                        new List<IField>
                        {
                            new Field("A", typeof(int), 1),
                            new Field("B", typeof(int), 1),
                            null
                        }
                    }
                });
        }

        [Test]
        public void ResultCountShouldMatchInput()
        {
            Assert.AreEqual(2,
                new Record(new List<List<List<IField>>> { 
                        new List<List<IField>>(), 
                        new List<List<IField>>() }).ResultCount);
        }

        [Test]
        public void RecordCountShouldMatchInput()
        {
            Assert.AreEqual(2,
                new Record(new List<List<List<IField>>> { 
                        new List<List<IField>> {
                            new List<IField>(),
                            new List<IField>()
                        }}).RecordCount);
        }

        [Test]
        public void ResultIndexShouldIncrement()
        {
            var r = (IRecord)new Record(new List<List<List<IField>>> { 
                        new List<List<IField>>(), 
                        new List<List<IField>>() });
            Assert.AreEqual(0, r.ResultIndex);
            r = r.NextResult;
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.ResultIndex);

        }

        [Test]
        public void RecordIndexShouldIncrement()
        {
            var r = (IRecord)new Record(new List<List<List<IField>>> { new List<List<IField>> { 
                    new List<IField> { new Field("A", typeof(int), 1) },
                    new List<IField> { new Field("A", typeof(int), 1) }} });
            Assert.AreEqual(0, r.RecordIndex);
            r = r.NextRecord;
            Assert.IsNotNull(r);
            Assert.AreEqual(1, r.RecordIndex);

        }

        [Test]
        public void FieldCountShouldMatchInput()
        {
            var r = (IRecord)new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1) } } });
            Assert.AreEqual(1, r.FieldCount);
        }

        [Test]
        public void ShouldBeAbleToGetNextResultFromFirstRecordWhenThereIsMoreThanOneRecord()
        {
            var r = (IRecord)new Record(new List<List<List<IField>>>
                {
                    new List<List<IField>>
                    {
                        new List<IField>
                        {
                            new Field("A", typeof(int), 1),
                            new Field("B", typeof(int), 1),
                            new Field("C", typeof(int), 1)
                        },                
                        new List<IField>
                        {
                            new Field("A", typeof(int), 1),
                            new Field("B", typeof(int), 1),
                            new Field("C", typeof(int), 1)
                        }
                    },  
                    new List<List<IField>>
                    {
                        new List<IField>
                        {
                            new Field("A", typeof(int), 1),
                            new Field("B", typeof(int), 1),
                            new Field("C", typeof(int), 1)
                        },                
                        new List<IField>
                        {
                            new Field("A", typeof(int), 1),
                            new Field("B", typeof(int), 1),
                            new Field("C", typeof(int), 1)
                        }
                    }
                });
            Assert.AreEqual(2, r.ResultCount);
            Assert.AreEqual(2, r.RecordCount);
            Assert.AreEqual(3, r.FieldCount);
            var x = r.NextRecord;
            Assert.IsNotNull(x);
            Assert.AreEqual(2, x.ResultCount);
            Assert.AreEqual(2, x.RecordCount);
            Assert.AreEqual(3, x.FieldCount);
            var y = r.NextResult;
            Assert.IsNotNull(y);
            Assert.AreEqual(2, y.ResultCount);
            Assert.AreEqual(2, y.RecordCount);
            Assert.AreEqual(3, y.FieldCount);
            var z = y.NextRecord;
            Assert.IsNotNull(z);
            Assert.AreEqual(2, z.ResultCount);
            Assert.AreEqual(2, z.RecordCount);
            Assert.AreEqual(3, z.FieldCount);
        }

        [Test]
        [ExpectedException(typeof(FieldMissingException))]
        public void GetValueShouldThrowWhenPassedNegativeIndex()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(-1);
        }

        [Test]
        [ExpectedException(typeof(FieldMissingException))]
        public void GetValueShouldThrowWhenPassedIndexThatIsGreaterThanFieldCount()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(3);
        }

        [Test]
        public void GetValueByIndexShouldReturnANonNullValue()
        {
            Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(0));
        }

        [Test]
        public void GetValueByIndexShouldMatchInput()
        {
            Assert.AreEqual(1, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(0));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetValueByNameShouldThrowOnNull()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(null);
        }

        [Test]
        [ExpectedException(typeof(FieldMissingException))]
        public void GetValueByNameShouldThrowOnInvalidName()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>("C");
        }

        [Test]
        public void GetValueByNameShouldNotReturnANullValue()
        {
            Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>("A"));
        }

        [Test]
        public void GetValueByNameShouldMatchInput()
        {
            Assert.AreEqual(1, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>("A"));
        }

        [Test]
        [ExpectedException(typeof(FieldMissingException))]
        public void GetFieldShouldThrowWhenPassedNegativeIndex()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetField(-1);
        }

        [Test]
        [ExpectedException(typeof(FieldMissingException))]
        public void GetFieldShouldThrowWhenPassedIndexThatIsGreaterThanFieldCount()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetField(3);
        }

        [Test]
        public void GetFieldByIndexShouldReturnANonNullValue()
        {
            Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetField(0));
        }

        [Test]
        public void GetFieldByIndexShouldMatchInput()
        {
            var f = new Field("A", typeof(int), 1);
            Assert.AreEqual(f, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { f, new Field("B", typeof(int), 2) } } }).GetField(0));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFieldByNameShouldThrowOnNull()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField>() } }).GetField(null);
        }

        [Test]
        [ExpectedException(typeof(FieldMissingException))]
        public void GetFieldByNameShouldThrowOnInvalidName()
        {
            new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField>() } }).GetField("C");
        }

        [Test]
        public void GetFieldByNameShouldNotReturnANullValue()
        {
            Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1) } } }).GetField("A"));
        }

        [Test]
        public void GetFieldByNameShouldMatchInput()
        {
            var f = new Field("A", typeof(int), 1);
            Assert.AreEqual(f, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { f } } }).GetField("A"));
        }

        [Test]
        public void GenericGetEnumeratorShouldMatchInput()
        {
            var f = new Field("A", typeof(int), 1);
            var r = (IRecord)new Record(new List<List<List<IField>>> { new List<List<IField>> { 
                    new List<IField> { f } } });
            using (var e = r.GetEnumerator())
            {
                Assert.IsNotNull(e);
                Assert.IsTrue(e.MoveNext());
                Assert.AreEqual(f, e.Current);
                Assert.IsFalse(e.MoveNext());
            }
        }

        [Test]
        public void GetEnumeratorShouldMatchInput()
        {
            var f = new Field("A", typeof(int), 1);
            var r = (IRecord)new Record(new List<List<List<IField>>> { new List<List<IField>> { 
                    new List<IField> { f } } });
            var e = ((IEnumerable)r).GetEnumerator();
            Assert.IsNotNull(e);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(f, e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void ShouldAllowDynamicFieldAccess()
        {
            var f1 = new Field("A", typeof(int), 1);
            var f2 = new Field("B", typeof(int), 2);
            dynamic r = (IRecord)new Record(new List<List<List<IField>>> { new List<List<IField>> { 
                    new List<IField> { f1, f2 } } });
            Assert.AreEqual(f1.Value, r.A);
            Assert.AreEqual(f2.Value, r.B);
        }

        [Test]
        public void Method_GetName()
        {
            Assert.Throws<FieldMissingException>(() => _1RES_1REC_2FLD.GetName(-1));
            Assert.Throws<FieldMissingException>(() => _1RES_1REC_2FLD.GetName(3));
            Assert.AreEqual("A", _1RES_1REC_2FLD.GetName(0));
            Assert.AreEqual("B", _1RES_1REC_2FLD.GetName(1));
        }

        [Test]
        public void Method_GetType_ByIndex()
        {
            Assert.Throws<FieldMissingException>(() => _1RES_1REC_2FLD.GetType(-1));
            Assert.Throws<FieldMissingException>(() => _1RES_1REC_2FLD.GetType(3));
            Assert.AreEqual(typeof(int), _1RES_1REC_2FLD.GetType(0));
            Assert.AreEqual(typeof(string), _1RES_1REC_2FLD.GetType(1));
        }

        [Test]
        public void Method_GetType_ByName()
        {
            Assert.Throws<ArgumentNullException>(() => _1RES_1REC_2FLD.GetType(null));
            Assert.Throws<FieldMissingException>(() => _1RES_1REC_2FLD.GetType("C"));
            Assert.AreEqual(typeof(int), _1RES_1REC_2FLD.GetType("A"));
            Assert.AreEqual(typeof(string), _1RES_1REC_2FLD.GetType("B"));
        }

        [Test]
        public void Method_GetFieldNames()
        {
            Assert.IsNotNull(_1RES_1REC_2FLD.GetFieldNames());
            Assert.AreEqual("A", _1RES_1REC_2FLD.GetFieldNames().First());
            Assert.AreEqual("B", _1RES_1REC_2FLD.GetFieldNames().Last());
        }

        [Test]
        public void EnumerateRecords()
        {
            using (var e = _1RES_2REC_2FLD.EnumerateRecords().GetEnumerator())
            {
                for (int i = 0; i < _1RES_2REC_2FLD.RecordCount; i++)
                {
                    Assert.IsTrue(e.MoveNext());
                    Assert.IsNotNull(e.Current);
                }
            }
        }

        private static readonly Expression _expression = Expression.Parameter(typeof(object));

        [Test]
        public void RecordDynamicMetaObject_Constructors()
        {
            Assert.Throws<ArgumentNullException>(() => new Record.RecordDynamicMetaObject(null, _1RES_1REC_2FLD));
            Assert.Throws<ArgumentNullException>(() => new Record.RecordDynamicMetaObject(_expression, null));
            new Record.RecordDynamicMetaObject(_expression, _1RES_1REC_2FLD);
        }

        [Test]
        public void RecordDynamicMetaObject_Method_GetDynamicMemberNames()
        {
            var r = new Record.RecordDynamicMetaObject(_expression, _1RES_1REC_2FLD);
            Assert.IsNotNull(r.GetDynamicMemberNames());
            Assert.IsTrue(r.GetDynamicMemberNames().SequenceEqual(_1RES_1REC_2FLD.GetFieldNames()));
        }

        [Test]
        public void RecordDynamicMetaObject_Method_BindGetMember()
        {
            var r = new Record.RecordDynamicMetaObject(_expression, _1RES_1REC_2FLD);
            Assert.Throws<ArgumentNullException>(() => r.BindGetMember(null));
        }
    }
}
