using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.SQLite
{
    [TestClass]
    public sealed class SQLiteColorTest
    {
        static string _dbFilePath;
        static MData.SQLite.SQLiteDatabase _db;

        static readonly Color Red = new Color(255, 0, 0, 0);
        static readonly Color Green = new Color(0, 255, 0, 0);
        static readonly Color Blue = new Color(0, 0, 255, 0);
        static readonly Color Yellow = new Color(255, 255, 0, 0);
        static readonly Color[] Colors = new[] {
            Red, Green, Blue, Yellow
        };

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _dbFilePath = Path.GetTempFileName() + ".db";
            SQLiteConnection.CreateFile(_dbFilePath);
            _db = new SQLiteDatabase("Data Source=" + _dbFilePath);
            _db.BuildTextCommand(@"
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
            ").Execute();
        }

        [TestMethod]
        public void SQLiteColorTest_Execute()
        {
            InsertColor(Red);
            InsertColor(Green);
            InsertColor(Blue);
            InsertColor(Yellow);
            DeleteColor(Yellow);
            InsertColor(Yellow);
        }

        [TestMethod]
        public void SQLiteColorTest_ExecuteScalar()
        {
            Assert.AreEqual(4, _db.BuildTextCommand("Select Count(*) From Color").Execute<long>());
        }

        [TestMethod]
        public void SQLiteColorTest_ExecuteResults()
        {
            var result = _db.BuildTextCommand("Select * From Color").ExecuteResults();
            var foundColors = result.EnumerateRecords()
                                .Select(x => {
                                    dynamic r = x;
                                    return new Color(r.R, r.G, r.B, r.A);
                                }).ToArray();
            Assert.AreEqual(4, foundColors.Length);
            CollectionAssert.Contains(foundColors, Red);
            CollectionAssert.Contains(foundColors, Green);
            CollectionAssert.Contains(foundColors, Blue);
            CollectionAssert.Contains(foundColors, Yellow);
        }

        [ClassCleanup]
        public static void TestCleanup()
        {
            if (File.Exists(_dbFilePath))
            {
                File.Delete(_dbFilePath);
            }
        }

        public void InsertColor(Color color)
        {
            _db.BuildTextCommand("Insert Into Color (R,G,B,A) Values (@R,@G,@B,@A);", color).Execute();
        }

        public void DeleteColor(Color color)
        {
            _db.BuildTextCommand("Delete From Color Where R = @R And B = @B And G = @G And A = @A", color).Execute();
        }

        public struct Color
        {
            private readonly byte _r;
            private readonly byte _g;
            private readonly byte _b;
            private readonly byte _a;

            public Color(byte r, byte g, byte b, byte a)
            {
                _r = r;
                _g = g;
                _b = b;
                _a = a;
            }

            public int R
            {
                get { return _r; }
            }

            public int G
            {
                get { return _g; }
            }

            public int B
            {
                get { return _b; }
            }

            public int A
            {
                get { return _a; }
            }

            public override string ToString()
            {
                return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", _r, _g, _b, _a);
            }
        }
    }
}
