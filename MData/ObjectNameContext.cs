using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace MData
{
    public sealed class ObjectNameContext : DynamicObject
    {
        private readonly Database _db;
        private readonly string _name;

        public ObjectNameContext(Database db, string name = null)
        {
            _db = db;
            _name = name ?? "";
        }

        public ObjectNameContext EnterChildContext(string name)
        {
            name.ThrowIfNullOrWhiteSpace("name");
            return new ObjectNameContext(_db, _db.Provider.CombineObjectName(_name, name));
        }

        public Reader Exec(string name, object args = null)
        {
            return _db.ExecReader(name, args);
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
            result = EnterChildContext(binder.Name);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            if (_name.Length == 0)
                throw new Exception();
            result = Exec(_name, binder.CallInfo.ArgumentNames, args);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Exec(_db.Provider.CombineObjectName(_name, binder.Name), binder.CallInfo.ArgumentNames, args);
            return true;
        }

        private object Exec(string name, ReadOnlyCollection<string> names, object[] args)
        {
            var range = Enumerable.Range(0, args.Length);
            var argMap =
                Enumerable.Range(0, args.Length).ToDictionary(
                    i => _db.Provider.FormatParameterName(i < names.Count ? names[i] : "arg" + i),
                    i => args[i]);
            var text = _db.Provider.FormatProcedureCall(name, args.Length, i => i < names.Count ? names[i] : null);
            return _db.ExecReader(text, argMap);
        }
    }
}
