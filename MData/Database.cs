using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections;
using MData.Providers;

namespace MData
{
    public delegate QueryContext ContextSelector(dynamic ctx);

    public delegate QueryContext ContextConfigure(QueryContext ctx); 

	public sealed class Database
	{
        private readonly IProvider _provider;
        private readonly QueryContext _context;


        internal Database(IProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            _provider = provider;
            _context = new QueryContext(this);
        }

        
        internal IProvider Provider
        {
            get { return _provider; }
        }

        public Reader ExecReader(ContextSelector selector, ContextConfigure configure = null)
        {
            selector.ThrowIfNull("selector");
            var ctx = selector(_context);
            if (ctx == null)
                throw new Exception();
            if (configure != null)
            {
                ctx = configure(ctx);
                if (ctx == null)
                    throw new Exception();
            }
            return ctx.Exec();
        }


		public Reader ExecReader(string text, object args = null)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			return new Reader(_provider.CreateCommand(text, args).ExecuteReader(CommandBehavior.CloseConnection));
		}

		public ResultSet ExecResultSet(string text, object args = null)
		{
			using (var r = ExecReader(text, args))
				return new ResultSet(r);
		}

		public RecordSet ExecRecordSet(string text, object args = null)
		{
			using (var r = ExecReader(text, args))
				return new RecordSet(r);
		}

		public FieldSet ExecFieldSet(string text, object args = null)
		{
			using (var r = ExecReader(text, args))
			{
				if (!r.ReadRecord())
					throw new NoRecordException();
				return new FieldSet(r);
			}
		}

		public T Exec<T>(string text, object args = null)
		{
			using (var r = ExecReader(text, args))
			{
				if (!r.ReadRecord())
					throw new NoRecordException();
				return r.ReadField<T>(0);
			}
		}

		public void Exec(string text, object args = null)
		{
			using (var r = ExecReader(text, args))
				return;
		}
		
		public T BindEntity<T>(Func<T> create, string text, object args = null)
		{
			if (create == null)
				throw new ArgumentNullException("create");
			var instance = create();
			if (object.ReferenceEquals(instance, null))
				throw new Exception("instance");
			var fs = ExecFieldSet(text, args);
			foreach (var f in fs)
			{
				var prop = typeof(T).GetProperty(f.Name);
				if (prop == null)
					continue;
				prop.SetValue(instance, f.Value, null);
			}
			return instance;
		}

		public ReadOnlyCollection<T> BindEntityCollection<T>(Func<T> create, string text, object args = null, object options = null)
		{
			if (create == null)
				throw new ArgumentNullException("create");
			var es = new List<T>();
			foreach (var record in ExecRecordSet(text, args))
			{
				var instance = create();
				if (object.ReferenceEquals(instance, null))
					throw new Exception("instance");
				foreach (var field in record)
				{
					var prop = typeof(T).GetProperty(field.Name);
					if (prop == null)
						continue;
					prop.SetValue(instance, field.Value, null);
				}
				es.Add(instance);
			}
			return es.AsReadOnly();
		}
		
		public static Database GetSqlServerInstance(string connectionString, int timeout = 0)
		{
            return new Database(new SqlServerProvider(connectionString, timeout));
		}

        public static Database GetSqlServerCompactInstance(string connectionString)
        {
            return new Database(new SqlServerCompactProvider(connectionString));
        }
		
    }
}