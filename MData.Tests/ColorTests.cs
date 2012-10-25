using MData.SqlCe;
using MData.Tests.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MData.SQLite
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ColorTests
    {
        const string _createTableSql = @"
                Create Table Color (
                    R tinyint Not Null,
                    G tinyint Not Null,
                    B tinyint Not Null,
                    A tinyint Not Null,
                    Constraint PK_Color Primary Key (
                        R,
                        G,
                        B,
                        A
                    )
                )
            ";

        static string _sqliteDBPath = Path.GetTempPath() + "Test.db";
        static IDatabase _sqliteDB = Databases.GetSQLiteDatabase("Data Source=\"" + _sqliteDBPath + "\";Version=3;New=True;");
        static string _sqlceDBPath = Path.GetTempPath() + "Test.sdf";
        static IDatabase _sqlceDB = Databases.GetSqlCeDatabase("Data Source=\"" + _sqlceDBPath + "\";Persist Security Info=False;");


        [TestFixtureSetUp]
        public static void Setup()
        {
            TearDown();
            SqlCeDatabase.CreateDatabaseFile(_sqlceDB.ConnectionString);
            _sqliteDB.BuildTextCommand(_createTableSql).Execute();
            _sqlceDB.BuildTextCommand(_createTableSql).Execute();
        }

        [Test]
        public void TestColorModel()
        {
            var c1 = new Color();
            Assert.AreEqual(c1, default(Color));
            Assert.IsTrue(c1 == default(Color));
            Assert.IsFalse(c1 != default(Color));
            Assert.IsTrue(((object)c1).Equals((object)default(Color)));
            Assert.AreEqual("00000000", c1.ToString());
            var c2 = new Color(255, 0, 0, 0);
            Assert.AreEqual(c2.R, 255);
            Assert.AreEqual(c2.G, 0);
            Assert.AreEqual(c2.B, 0);
            Assert.AreEqual(c2.A, 0);
            Assert.AreEqual(Color.Red.ToString(), c2.ToString());
            Assert.AreEqual(Color.Red.GetHashCode(), c2.GetHashCode());
        }

        [Test]
        public void SQlite_InsertColors()
        {
            InsertColors(_sqliteDB);
        }

        [Test]
        public void SqlCe_InsertColors()
        {
            InsertColors(_sqlceDB);
        }

        private static void InsertColors(IDatabase db)
        {
            InsertColor(db, Color.Red);
            InsertColor(db, Color.Green);
            InsertColor(db, Color.Blue);
            InsertColor(db, Color.Yellow);
        }

        private static void InsertColor(IDatabase db, Color color)
        {
            db.BuildTextCommand("Insert Into Color (R,G,B,A) Values (@R,@G,@B,@A);", color).Execute();
        }

        private static void DeleteColor(IDatabase db, Color color)
        {
            db.BuildTextCommand("Delete From Color Where R = @R And B = @B And G = @G And A = @A", color).Execute();
        }

        [TestFixtureTearDown]
        public static void TearDown()
        {
            try
            {
                if (File.Exists(_sqliteDBPath))
                    File.Delete(_sqliteDBPath);
                if (File.Exists(_sqlceDBPath))
                    File.Delete(_sqlceDBPath);
            }
            catch 
            {

            }
        }
    }
}