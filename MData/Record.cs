using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestClass]
        [ExcludeFromCodeCoverage]
        public sealed class Tests : TestsBase
        {

            private static readonly Record _1RES_1REC_2FLD = 
                new Record(
                    new List<List<List<IField>>> { 
                            new List<List<IField>> { 
                                new List<IField> { 
                                    new Field("A", typeof(int), 1), 
                                    new Field("B", typeof(string), "X") } } });


            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowOnNullResults() 
            { 
                new Record(null); 
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowOnNullRecords() 
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>>(), null }); 
            }

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
            public void ResultCountShouldMatchInput()
            {
                Assert.AreEqual(2,
                    new Record(new List<List<List<IField>>> { 
                        new List<List<IField>>(), 
                        new List<List<IField>>() }).ResultCount);
            }

            [TestMethod]
            public void RecordCountShouldMatchInput()
            {
                Assert.AreEqual(2,
                    new Record(new List<List<List<IField>>> { 
                        new List<List<IField>> {
                            new List<IField>(),
                            new List<IField>()
                        }}).RecordCount);
            }

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
            public void FieldCountShouldMatchInput()
            {
                var r = (IRecord)new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1) } } });
                Assert.AreEqual(1, r.FieldCount);
            }

            [TestMethod]
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

            [TestMethod]
            [ExpectedException(typeof(FieldMissingException))]
            public void GetValueShouldThrowWhenPassedNegativeIndex()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(-1);
            }

            [TestMethod]
            [ExpectedException(typeof(FieldMissingException))]
            public void GetValueShouldThrowWhenPassedIndexThatIsGreaterThanFieldCount()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(3);
            }

            [TestMethod]
            public void GetValueByIndexShouldReturnANonNullValue()
            {
                Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(0));
            }

            [TestMethod]
            public void GetValueByIndexShouldMatchInput()
            {
                Assert.AreEqual(1, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(0));
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void GetValueByNameShouldThrowOnNull()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>(null);
            }

            [TestMethod]
            [ExpectedException(typeof(FieldMissingException))]
            public void GetValueByNameShouldThrowOnInvalidName()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>("C");
            }

            [TestMethod]
            public void GetValueByNameShouldNotReturnANullValue()
            {
                Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>("A"));
            }

            [TestMethod]
            public void GetValueByNameShouldMatchInput()
            {
                Assert.AreEqual(1, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetValue<int>("A"));
            }

            [TestMethod]
            [ExpectedException(typeof(FieldMissingException))]
            public void GetFieldShouldThrowWhenPassedNegativeIndex()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetField(-1);
            }

            [TestMethod]
            [ExpectedException(typeof(FieldMissingException))]
            public void GetFieldShouldThrowWhenPassedIndexThatIsGreaterThanFieldCount()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetField(3);
            }

            [TestMethod]
            public void GetFieldByIndexShouldReturnANonNullValue()
            {
                Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1), new Field("B", typeof(int), 2) } } }).GetField(0));
            }

            [TestMethod]
            public void GetFieldByIndexShouldMatchInput()
            {
                var f = new Field("A", typeof(int), 1);
                Assert.AreEqual(f, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { f, new Field("B", typeof(int), 2) } } }).GetField(0));
            }

            [TestMethod]
            [ExpectedException(typeof(ArgumentNullException))]
            public void GetFieldByNameShouldThrowOnNull()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField>() } }).GetField(null);
            }

            [TestMethod]
            [ExpectedException(typeof(FieldMissingException))]
            public void GetFieldByNameShouldThrowOnInvalidName()
            {
                new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField>() } }).GetField("C");
            }

            [TestMethod]
            public void GetFieldByNameShouldNotReturnANullValue()
            {
                Assert.IsNotNull(new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { new Field("A", typeof(int), 1) } } }).GetField("A"));
            }

            [TestMethod]
            public void GetFieldByNameShouldMatchInput()
            {
                var f = new Field("A", typeof(int), 1);
                Assert.AreEqual(f, new Record(new List<List<List<IField>>> { new List<List<IField>> { new List<IField> { f } } }).GetField("A"));
            }

            [TestMethod]
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

            [TestMethod]
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

            [TestMethod]
            public void ShouldAllowDynamicFieldAccess()
            {
                var f1 = new Field("A", typeof(int), 1);
                var f2 = new Field("B", typeof(int), 2);
                dynamic r = (IRecord)new Record(new List<List<List<IField>>> { new List<List<IField>> { 
                    new List<IField> { f1, f2 } } });
                Assert.AreEqual(f1, r.A);
                Assert.AreEqual(f2, r.B);
            }

            [TestMethod]
            public void Method_GetName()
            {
                DoesThrow<FieldMissingException>(() => _1RES_1REC_2FLD.GetName(-1));
                DoesThrow<FieldMissingException>(() => _1RES_1REC_2FLD.GetName(3));
                Assert.AreEqual("A", _1RES_1REC_2FLD.GetName(0));
                Assert.AreEqual("B", _1RES_1REC_2FLD.GetName(1));
            }

            [TestMethod]
            public void Method_GetType_ByIndex()
            {
                DoesThrow<FieldMissingException>(() => _1RES_1REC_2FLD.GetType(-1));
                DoesThrow<FieldMissingException>(() => _1RES_1REC_2FLD.GetType(3));
                Assert.AreEqual(typeof(int), _1RES_1REC_2FLD.GetType(0));
                Assert.AreEqual(typeof(string), _1RES_1REC_2FLD.GetType(1));
            }

            [TestMethod]
            public void Method_GetType_ByName()
            {
                DoesThrow<ArgumentNullException>(() => _1RES_1REC_2FLD.GetType(null));
                DoesThrow<FieldMissingException>(() => _1RES_1REC_2FLD.GetType("C"));
                Assert.AreEqual(typeof(int), _1RES_1REC_2FLD.GetType("A"));
                Assert.AreEqual(typeof(string), _1RES_1REC_2FLD.GetType("B"));
            }

            [TestMethod]
            public void Method_GetFieldNames()
            {
                Assert.IsNotNull(_1RES_1REC_2FLD.GetFieldNames());
                Assert.AreEqual("A", _1RES_1REC_2FLD.GetFieldNames().First());
                Assert.AreEqual("B", _1RES_1REC_2FLD.GetFieldNames().Last());
            }
        }
    }
}