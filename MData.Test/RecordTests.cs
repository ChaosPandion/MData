using System;
using System.Linq;
using MData.Foundation;
using NUnit.Framework;

namespace MData.Test
{
	[TestFixture]
	class RecordTests
	{
		private static readonly IField _intField = new Field("A", typeof(int), 1);
		private static readonly IField _stringField = new Field("B", typeof(string), "X");
		private static readonly IField[] _fields = new[] { _intField, _stringField };

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldThrowOnNullFields() { new Record(null); }

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ShouldThrowWhenFieldsContainsANullField() { new Record(new[] { _intField, null }); }

		[Test]
		public void CountShouldMatchTheLengthOfFields() { Assert.AreEqual(new Record(_fields).Count, _fields.Length); }

		[Test]
		[ExpectedException(typeof(FieldMissingException))]
		public void ShouldThrowWhenNegativeIndexIsPassedToGetField() { new Record(_fields).GetField(-1); }

		[Test]
		[ExpectedException(typeof(FieldMissingException))]
		public void ShouldThrowWhenIndexPassedToGetFieldGreaterThanOrEqualToCount() { new Record(_fields).GetField(2); }

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ShouldThrowWhenNamePassedToGetFieldIsNull() { new Record(_fields).GetField(null); }

		[Test]
		[ExpectedException(typeof(FieldMissingException))]
		public void ShouldThrowWhenNamePassedToGetFieldIsNotAValidFieldName() { new Record(_fields).GetField("Q"); }

		[Test]
		public void FieldsShouldMatchInput() 
		{ 
			var r = new Record(_fields);
			Assert.AreEqual(r.GetField(0), _intField);
			Assert.AreEqual(r.GetField("A"), _intField);
			Assert.AreEqual(r.GetField(1), _stringField);
			Assert.AreEqual(r.GetField("B"), _stringField); 
		}

		[Test]
		public void FieldsShouldBeAccessibleDynamically()
		{
			dynamic r = new Record(_fields);
			Assert.AreEqual(r.A.Value, _intField.Value);
			Assert.AreEqual(r.B.Value, _stringField.Value);
		}

		[Test]
		public void FieldsShouldEnumerateInOrder()
		{
			var r = new Record(_fields);
			Assert.AreEqual(r.First(), _intField);
			Assert.AreEqual(r.Last(), _stringField);
		}
	}
}
