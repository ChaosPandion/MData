using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.SQLite
{
    public sealed class SQLiteCommand : Command<System.Data.SQLite.SQLiteConnection>
    {
        public SQLiteCommand(Database<System.Data.SQLite.SQLiteConnection> db)
            : base(db)
        {

        }

        protected override IDbCommand CreateCommand()
        {
            var cn = new System.Data.SQLite.SQLiteConnection();
            cn.ConnectionString = Database.ConnectionString;
            var cm = cn.CreateCommand();
            cm.CommandText = CommandText;
            if (CommandTimeout > -1)
            {
                cm.CommandTimeout = CommandTimeout;
            }
            foreach (var arg in CommandParameters)
            {
                var cmp = cm.CreateParameter();
                cmp.ParameterName = arg.Key;
                cmp.Value = arg.Value ?? DBNull.Value;
                cm.Parameters.Add(cmp);
            }
            cn.Open();
            return cm;
        }
    }
}
