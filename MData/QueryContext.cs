using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace MData
{
    public sealed class QueryContext : DynamicObject
    {
        private readonly Database _db;
        private readonly string _name;
        private readonly IDictionary<string, object> _args;

        internal QueryContext(Database db, string name = null, IDictionary<string, object> args = null)
        {
            _db = db;
            _name = name ?? "";
            _args = args;
        }

        public QueryContext Configure(int timeout = 0)
        {
            return new QueryContext(_db, _name, _args);
        }

        internal QueryContext EnterChildContext(string name)
        {
            name.ThrowIfNullOrWhiteSpace("name");
            return new QueryContext(_db, _db.Provider.CombineObjectName(_name, name));
        }

        public Reader Exec()
        {
            return _db.ExecReader(_name, _args);
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length != 1)
                throw new Exception();
            if (indexes[0] == null)
                throw new Exception();
            var name = (indexes[0] ?? "").ToString();
            result = EnterChildContext(name);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!_db.Provider.SupportsChainedContext)
                throw new Exception();
            result = EnterChildContext(binder.Name);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            if (_name.Length == 0)
                throw new Exception();
            if (_args != null)
                throw new Exception();
            result = new QueryContext(_db, _name, Args(_name, binder.CallInfo.ArgumentNames, args));
            return true;
        }
        
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if (!_db.Provider.SupportsChainedContext)
                throw new Exception();
            result = new QueryContext(_db, _db.Provider.CombineObjectName(_name, binder.Name), Args(_name, binder.CallInfo.ArgumentNames, args));
            return true;
        }

        private IDictionary<string, object> Args(string name, ReadOnlyCollection<string> names, object[] args)
        {
            var argMap =
                Enumerable.Range(0, args.Length).ToDictionary(
                    i => _db.Provider.FormatParameterName(i < names.Count ? names[i] : "arg" + i),
                    i => args[i]);
            return argMap;
            //var argMap = _db.Provider.FormatProcedureCall(name, args.Length, i => i < names.Count ? names[i] : null);
            //return _db.ExecReader(text, argMap);
        }
    }
}
