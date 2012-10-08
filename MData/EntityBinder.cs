using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public static class EntityBinder
    {
        public static T BindEntity<T>(this Database db, Func<T> create, string text, object args = null, object options = null)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            if (create == null)
                throw new ArgumentNullException("create");
            var instance = create();
            if (object.ReferenceEquals(instance, null))
                throw new Exception("instance");
            var fs = db.ExecFieldSet(text, args, options);
            foreach (var f in fs)
            {
                var prop = typeof(T).GetProperty(f.Name);
                if (prop == null)
                    continue;
                prop.SetValue(instance, f.Value, null);
            }
            return instance;
        }

        public static ReadOnlyCollection<T> BindEntityCollection<T>(this Database db, Func<T> create, string text, object args = null, object options = null)
        {
            if (db == null)
                throw new ArgumentNullException("db");
            if (create == null)
                throw new ArgumentNullException("create");
            var es = new List<T>();
            foreach (var record in db.ExecRecordSet(text, args, options))
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
    }
}