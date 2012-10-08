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

		public Reader ExecReader(string text, object args = null, object options = null)
		{
			if (string.IsNullOrWhiteSpace(text))
				throw new ArgumentException("text");
			var ado = new ADO();
			ado.Connection = _createConnection();
			ado.Connection.Open();
			ado.Command = ado.Connection.CreateCommand();
			ado.Command.CommandText = text;
			ado.Command.CommandType = CommandType.StoredProcedure;
			if (args != null)
			{
				var ps = args.GetType().GetProperties();
				foreach (var p in ps)
				{
					if (!p.CanRead)
						continue;
					var cmp = ado.Command.CreateParameter();
					cmp.ParameterName = p.Name;
					cmp.Value = p.GetValue(args, null) ?? DBNull.Value;
					ado.Command.Parameters.Add(cmp);
				}
			}
			if (options != null)
			{
				var map = options.GetType().GetProperties().ToDictionary(p => p.Name);
				if (map.ContainsKey("Text"))
				{
					ado.Command.CommandType = CommandType.Text;
				}
			}
			ado.Reader = ado.Command.ExecuteReader();
			return new Reader(ado);
		}

        public ResultSet ExecResultSet(string text, object args = null, object options = null)
        {
			using (var r = ExecReader(text, args, options))
				return new ResultSet(r);
        }

        public RecordSet ExecRecordSet(string text, object args = null, object options = null)
        {
			using (var r = ExecReader(text, args, options))
				return new RecordSet(r);
        }

        public FieldSet ExecFieldSet(string text, object args = null, object options = null)
		{
			using (var r = ExecReader(text, args, options))
			{
				if (!r.ReadRecord())
					throw new NoRecordException();
				return new FieldSet(r);
			}
        }

        public T Exec<T>(string text, object args = null, object options = null)
        {
			var f = ExecFieldSet(text, args, options);
			return f.Get<T>(0);
        }

        public void Exec(string text, object args = null, object options = null)
        {
			using (ExecReader(text, args, options))
                return;
        }
    }
}