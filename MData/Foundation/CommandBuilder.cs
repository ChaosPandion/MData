using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MData.Support;

namespace MData.Foundation
{
    public abstract class CommandBuilder<TConnection> : DynamicObject, ICommandBuilder
        where TConnection : IDbConnection, new()
    {
        private readonly ConcurrentDictionary<string, object> _commandParameters = new ConcurrentDictionary<string, object>();
        private readonly Database<TConnection> _source;
        private string _commandText;
        private CommandType _commandType;
        private int _commandTimeout = -1;

        protected CommandBuilder(Database<TConnection> source)
        {
            _source = source;
        }

		public dynamic Procedures
		{
			get { return this; }
		}
        
        public ICommandBuilder SetText(string value)
        {
            _commandText = value;
            _commandType = CommandType.Text;
            return this;
        }

        public ICommandBuilder SetProcedure(string value)
        {
            _commandText = value;
            _commandType = CommandType.StoredProcedure;
            return this;
        }

        public ICommandBuilder SetTimeout(int value)
        {
            _commandTimeout = value;
            return this;
        }

        public ICommandBuilder AddArgument<T>(string name, T value)
        {
            _commandParameters.TryAdd(name, value);
            return this;
        }

        public ICommandBuilder AddArguments(IDictionary<string, object> args)
        {
            foreach (var kv in args)
                _commandParameters.TryAdd(kv.Key, kv.Value);
            return this;
        }

        public ICommandBuilder AddArguments<T>(T value)
        {
            Reflection.ForEachProperty(value, (k, v) => _commandParameters.TryAdd(k, v));
            return this;
		}

        public virtual void Execute()
        {
            using (var cm = CreateCommand())
                cm.ExecuteNonQuery();
        }

		public virtual T Execute<T>()
        {
			using (var reader = ExecuteReader())
            {
                if (!reader.ReadRecord())
                    throw new Exception();
                return reader.GetFieldValue<T>(0);
            }
        }

		public virtual IRecord ExecuteRecord()
        {
			using (var reader = ExecuteReader())
            {
                if (!reader.ReadRecord())
                    throw new Exception();
                return new Record(reader.GetFields());
            }
        }

		public virtual IRecordSet ExecuteRecords()
        {
			using (var reader = ExecuteReader())
            {
                var records = new List<IRecord>();
                while (reader.ReadRecord())
                    records.Add(new Record(reader.GetFields()));
				return new RecordSet(records);
            }
        }

		public virtual IResultSet ExecuteResults()
        {
			using (var reader = ExecuteReader())
            {
				var results = new List<IRecordSet>();
                do
                {
                    var records = new List<IRecord>();
                    while (reader.ReadRecord())
                        records.Add(new Record(reader.GetFields()));
                    results.Add(new RecordSet(records));
                } while (reader.ReadResult());
				return new ResultSet(results);
            }
        }

		public virtual IReader ExecuteReader()
        {
            return new Reader(CreateCommand().ExecuteReader(CommandBehavior.CloseConnection));
		}


		public virtual T ExecuteEntity<T>() 
			where T : new()
		{
			return RecordBinder<T>.Bind(ExecuteRecord(), () => new T());
		}

		public virtual T ExecuteEntity<T>(T entity)
		{
			return RecordBinder<T>.Bind(ExecuteRecord(), () => entity);
		}

		public virtual T ExecuteEntity<T>(Func<T> createInstance)
		{
			return RecordBinder<T>.Bind(ExecuteRecord(), createInstance);
		}

		public virtual IEnumerable<T> ExecuteEntityCollection<T>()
			where T : new()
		{
			return RecordSetBinder<T>.Bind(ExecuteRecords(), () => new T());
		}

		public virtual IEnumerable<T> ExecuteEntityCollection<T>(Func<T> createInstance)
		{
			createInstance.ThrowIfNull("createInstance");
			return RecordSetBinder<T>.Bind(ExecuteRecords(), createInstance);
		}

        protected virtual IDbCommand CreateCommand()
        {
            var cn = new TConnection();
            cn.ConnectionString = _source.ConnectionString;
            var cm = cn.CreateCommand();
            cm.CommandText = _commandText;
            cm.CommandType = _commandType;
            if (_commandTimeout != -1)
            {
                cm.CommandTimeout = _commandTimeout;
            }
            foreach (var arg in _commandParameters)
            {
                var cmp = cm.CreateParameter();
                cmp.ParameterName = arg.Key;
                cmp.Value = arg.Value ?? DBNull.Value;
                cm.Parameters.Add(cmp);
            }
            cn.Open();
            return cm;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            SetText(indexes[0].ToString());
            result = this;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            SetProcedure(_commandText != null ? _commandText + "." + binder.Name : binder.Name);
            result = this;
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            AddArgs(binder.CallInfo.ArgumentNames, args);
            result = this;
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            SetProcedure(_commandText != null ? _commandText + "." + binder.Name : binder.Name);
            AddArgs(binder.CallInfo.ArgumentNames, args);
            result = this;
            return true;
        }

        private void AddArgs(ReadOnlyCollection<string> names, object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                AddArgument(i < names.Count ? names[i] : "arg" + i, args[i]);
        }
	}
}