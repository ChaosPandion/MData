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

namespace MData
{
	public sealed class Database
	{
		private static readonly ConcurrentDictionary<Type, Action<object, Action<string, object>>> _argsTypeMap = new ConcurrentDictionary<Type, Action<object, Action<string, object>>>();
		private static readonly Regex _spRegex = new Regex(@"^\s*(?:\[?[a-zA-Z][a-zA-Z0-9]*\]?\.)?\[?[a-zA-Z][a-zA-Z0-9]*\]?\s*$", RegexOptions.Compiled);
		private readonly Func<string, object, IDataReader> _createReader;


        internal Database(Func<string, object, IDataReader> createReader)
        {
			if (createReader == null)
				throw new ArgumentNullException("createReader");
			_createReader = createReader;
        }
		

		public Reader ExecReader(string text, object args = null)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			return new Reader(_createReader(text, args));
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
			var csb = new SqlConnectionStringBuilder(connectionString);
			return new Database((text, args) =>
			{
				var cn = new SqlConnection(csb.ConnectionString);
				var cm = cn.CreateCommand();
				if (timeout > 0)
					cm.CommandTimeout = timeout;
				cm.CommandText = text;
				cm.CommandType = _spRegex.IsMatch(text) ? CommandType.StoredProcedure : CommandType.Text;
				if (args != null)
				{
					ReadArgs(args, (key, val) =>
					{
						var cmp = cm.CreateParameter();
						cmp.ParameterName = key;
						cmp.Value = val;
						cm.Parameters.Add(cmp);
					});
				}
				cn.Open();
				return cm.ExecuteReader(CommandBehavior.CloseConnection);
			});
		}

		private static void ReadArgs(object instance, Action<string, object> send)
		{
			if (instance == null)
				return;
			if (send == null)
				throw new ArgumentNullException("send");
			var type = instance.GetType();
			Action<object, Action<string, object>> result;
			if (_argsTypeMap.TryGetValue(type, out result))
			{
				result(instance, send);
				return;
			}
			lock (_argsTypeMap)
			{
				if (_argsTypeMap.TryGetValue(type, out result))
				{
					result(instance, send);
					return;
				}

				if (instance is IDictionary)
				{
					result = (i, s) =>
					{
						var dict = i as IDictionary;
						foreach (KeyValuePair<string, object> kv in dict)
							s(kv.Key, kv.Value);
					};
				}
				else if (instance is IEnumerable)
				{
					result = (i, s) =>
					{
						var seq = i as IEnumerable;
						foreach (object v in seq)
							s(null, v);
					};
				}
				else
				{
					var dbnull = Expression.Constant(DBNull.Value);
					var objArg = Expression.Parameter(typeof(object), "arg");
					var sendArg = Expression.Parameter(typeof(Action<string, object>), "send");
					var exps = new List<Expression>();
					var props = type.GetProperties();
					foreach (var prop in props)
					{
						if (!prop.CanRead)
							continue;
						var pname = Expression.Constant(prop.Name);
						var paccess = Expression.Property(Expression.Convert(objArg, type), prop);
						var pval = Expression.Coalesce(Expression.Convert(paccess, typeof(object)), dbnull);
						exps.Add(Expression.Invoke(sendArg, pname, pval));
					}
					var body = Expression.Block(exps);
					var lambda = Expression.Lambda<Action<object, Action<string, object>>>(body, objArg, sendArg);
					result = lambda.Compile();
				}
				_argsTypeMap.TryAdd(type, result);
				result(instance, send);
			}
		}
    }
}