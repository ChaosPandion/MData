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

namespace MData.Foundation
{
    public abstract class RequestBuilder<TConnection> : DynamicObject, IRequestBuilder
        where TConnection : IDbConnection, new()
    {
        private readonly ConcurrentDictionary<string, object> _commandParameters = new ConcurrentDictionary<string, object>();
        private readonly Source<TConnection> _source;
        private string _commandText;
        private CommandType _commandType;
        private int _commandTimeout;

        protected RequestBuilder(Source<TConnection> source)
        {
            _source = source;
        }
        
        public IRequestBuilder SetText(string value)
        {
            _commandText = value;
            _commandType = CommandType.Text;
            return this;
        }

        public IRequestBuilder SetProcedure(string value)
        {
            _commandText = value;
            _commandType = CommandType.StoredProcedure;
            return this;
        }

        public IRequestBuilder SetTimeout(int value)
        {
            _commandTimeout = value;
            return this;
        }

        public IRequestBuilder AddArgument<T>(string name, T value)
        {
            _commandParameters.TryAdd(name, value);
            return this;
        }

        public IRequestBuilder AddArguments(IDictionary<string, object> args)
        {
            foreach (var kv in args)
                _commandParameters.TryAdd(kv.Key, kv.Value);
            return this;
        }

        public IRequestBuilder AddArguments<T>(T value)
        {
            Reflection.ForEachProperty(value, (k, v) => _commandParameters.TryAdd(k, v));
            return this;
        }

        public virtual void Request()
        {
            using (var cm = CreateCommand())
                cm.ExecuteNonQuery();
        }

        public virtual T Request<T>()
        {
            using (var reader = RequestReader())
            {
                if (!reader.ReadRecord())
                    throw new Exception();
                return reader.GetFieldValue<T>(0);
            }
        }

        public virtual IRecord RequestRecord()
        {
            using (var reader = RequestReader())
            {
                if (!reader.ReadRecord())
                    throw new Exception();
                return new Record(reader.GetFields());
            }
        }

        public virtual IList<IRecord> RequestRecords()
        {
            using (var reader = RequestReader())
            {
                var records = new List<IRecord>();
                while (reader.ReadRecord())
                    records.Add(new Record(reader.GetFields()));
                return records.AsReadOnly();
            }
        }

        public virtual IList<IList<IRecord>> RequestResults()
        {
            using (var reader = RequestReader())
            {
                var results = new List<IList<IRecord>>();
                do
                {
                    var records = new List<IRecord>();
                    while (reader.ReadRecord())
                        records.Add(new Record(reader.GetFields()));
                    results.Add((IList<IRecord>)records.AsReadOnly());
                } while (reader.ReadResult());
                return results.AsReadOnly();
            }
        }

        public virtual IReader RequestReader()
        {
            return new Reader(CreateCommand().ExecuteReader(CommandBehavior.CloseConnection));
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
                cm.Parameters.Add(arg);
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