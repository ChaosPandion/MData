using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.SQLite
{
    public sealed class SQLiteDatabase : Database<System.Data.SQLite.SQLiteConnection>
    {
        public SQLiteDatabase(string connectionString)
            : base(connectionString)
        {

        }

        protected override ICommand CreateCommand()
        {
            return new SQLiteCommand(this);
        }
    }
}