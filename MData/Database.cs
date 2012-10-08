using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public sealed class Database
    {
        private readonly Func<IDbConnection> _createConnection;

        public Database(Func<IDbConnection> createConnection)
        {
            if (createConnection == null)
                throw new ArgumentNullException("createConnection");
            _createConnection = createConnection;
        }

        public static Database CreateForSqlServer(string connectionString)
        {
            var csb = new SqlConnectionStringBuilder(connectionString);
            return new Database(() => new SqlConnection(csb.ConnectionString));
        }

        public ResultSet ExecResultSet(string text, object args = null, object options = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("text");
            var ado = new ADO();
            ado.Connection = _createConnection();
            ado.Connection.Open();
            ado.Command = ado.Connection.CreateCommand();
            ado.Command.CommandText = text;
            if (args != null)
            {
                var ps = args.GetType().GetProperties();
                foreach (var p in ps)
                {
                    if (!p.CanRead)
                        continue;
                    var cmp = ado.Command.CreateParameter();
                    cmp.ParameterName = p.Name;
                    cmp.Value = p.GetValue(args);
                    ado.Command.Parameters.Add(cmp);
                }
            }
            if (options != null)
            {

            }
            ado.Reader = ado.Command.ExecuteReader();
            return new ResultSet(ado);
        }

        public RecordSet ExecRecordSet(string text, object args = null, object options = null)
        {
            foreach (var b in ExecResultSet(text, args, options))
                    return b;
            throw new Exception();
        }

        public FieldSet ExecFieldSet(string text, object args = null, object options = null)
        {
            using (var a = ExecResultSet(text, args, options))
                foreach (var b in a)
                    foreach (var c in b)
                        return c;
            throw new Exception();
        }

        public T Exec<T>(string text, object args = null, object options = null)
        {
            using (var a = ExecResultSet(text, args, options))
                foreach (var b in a)
                    foreach (var c in b)
                        foreach (var d in c)
                            return (T)d.Value;
            throw new Exception();
        }

        public void Exec(string text, object args = null, object options = null)
        {
            using (ExecResultSet(text, args, options))
                return;
        }
    }
}