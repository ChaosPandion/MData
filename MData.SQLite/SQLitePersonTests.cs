using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.SQLite
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class SQLitePersonTests
    {
        static string _dbFilePath;
        static MData.SQLite.SQLiteDatabase _db;

        [ClassInitialize]
        public static void Initialize(TestContext ctx)
        {
            _dbFilePath = Path.GetTempFileName() + ".db";
            SQLiteConnection.CreateFile(_dbFilePath);
            _db = new SQLiteDatabase("Data Source=" + _dbFilePath);
            _db.BuildCommand(c => c.WithText(@"
                Create Table Person As
                Select 'Matt' As FirstName, 'Simpson' As LastName, 23 As Age Union
                Select 'Dan' As FirstName, 'Simpson' As LastName, 21 As Age Union
                Select 'Steve' As FirstName, 'Thomas' As LastName, 19 As Age Union
                Select 'Tim' As FirstName, 'Frank' As LastName, 16 As Age
            ")).Execute();
        }

        [TestMethod]
        public void TheCorrectAmountOfPeopleIsReturned()
        {
            Assert.AreEqual(4L, _db.BuildCommand(c => c.WithText(@"Select Count(*) From Person")).Execute<long>());
        }

        [TestMethod]
        public void Test_ExecuteResults()
        {
            IRecord r = _db.BuildCommand(c => c.WithText(@"Select * From Person Order By FirstName Asc")).ExecuteResults();
            Assert.AreEqual(4, r.RecordCount);

            dynamic n = r;
            Assert.AreEqual("Dan", n.FirstName.Value);
            Assert.AreEqual("Simpson", n.LastName.Value);
            Assert.AreEqual(21, n.Age.Value);

            r = r.NextRecord;
            n = r;
            Assert.AreEqual("Matt", n.FirstName.Value);
            Assert.AreEqual("Simpson", n.LastName.Value);
            Assert.AreEqual(23, n.Age.Value);

            r = r.NextRecord;
            n = r;
            Assert.AreEqual("Steve", n.FirstName.Value);
            Assert.AreEqual("Thomas", n.LastName.Value);
            Assert.AreEqual(19, n.Age.Value);

            r = r.NextRecord;
            n = r;
            Assert.AreEqual("Tim", n.FirstName.Value);
            Assert.AreEqual("Frank", n.LastName.Value);
            Assert.AreEqual(16, n.Age.Value);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            if (File.Exists(_dbFilePath))
            {
                File.Delete(_dbFilePath);
            }
        }
    }
}
