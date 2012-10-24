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

namespace MData
{
    public class Command<TConnection> : DynamicObject, ICommand
        where TConnection : IDbConnection, new()
    {
        private readonly ConcurrentDictionary<string, object> _commandParameters = new ConcurrentDictionary<string, object>();
        private readonly Database<TConnection> _db;
        private string _commandText;
        private CommandType _commandType;
        private int _commandTimeout = -1;

        public Command(Database<TConnection> db)
        {
            _db = db;
        }

        protected IDatabase Database
        {
            get { return _db; }
        }

        protected string CommandText
        {
            get { return _commandText; }
        }

        protected CommandType CommandType
        {
            get { return _commandType; }
        }

        protected int CommandTimeout
        {
            get { return _commandTimeout; }
        }

        protected IDictionary<string, object> CommandParameters
        {
            get { return _commandParameters; }
        }
        
        public ICommand WithText(string value)
        {
            _commandText = value;
            return this;
        }

        public ICommand WithType(CommandType value)
        {
            _commandType = value;
            return this;
        }

        public ICommand WithTimeout(int value)
        {
            _commandTimeout = value;
            return this;
        }

        public ICommand WithParam<T>(string name, T value)
        {
            _commandParameters.TryAdd(name, value);
            return this;
        }

        public ICommand WithParams(IDictionary<string, object> args)
        {
            foreach (var kv in args)
                _commandParameters.TryAdd(kv.Key, kv.Value);
            return this;
        }

        public ICommand WithParams<T>(T value)
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
                return reader.GetValue<T>(0);
            }
        }

        public virtual IRecord ExecuteResults()
        {
            using (var reader = ExecuteReader())
            {
                var results = new List<List<List<IField>>>();
                do
                {
                    var records = new List<List<IField>>();
                    while (reader.ReadRecord())
                        records.Add(new List<IField>(reader));
                    results.Add(records);
                } while (reader.ReadResult());
                return new Record(results);
            }
        }

		public virtual IReader ExecuteReader()
        {
            return new Reader(CreateCommand().ExecuteReader(CommandBehavior.CloseConnection));
		}

        protected virtual IDbCommand CreateCommand()
        {
            var cn = new TConnection();
            cn.ConnectionString = _db.ConnectionString;
            var cm = cn.CreateCommand();
            cm.CommandText = _commandText;
            cm.CommandType = (System.Data.CommandType)_commandType;
            if (_commandTimeout > -1)
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
            WithText(indexes[0].ToString());
            result = this;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            _commandType = CommandType.StoredProcedure;
            WithText(_commandText != null ? _commandText + "." + binder.Name : binder.Name);
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
            _commandType = CommandType.StoredProcedure;
            WithText(_commandText != null ? _commandText + "." + binder.Name : binder.Name);
            AddArgs(binder.CallInfo.ArgumentNames, args);
            result = this;
            return true;
        }

        private void AddArgs(ReadOnlyCollection<string> names, object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                WithParam(i < names.Count ? names[i] : "arg" + i, args[i]);
        }
    }
}