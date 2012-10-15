using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MData
{
    static class Reflection
    {
        private static readonly ConcurrentDictionary<Type, Action<object, Action<string, object>>> _argsTypeMap = new ConcurrentDictionary<Type, Action<object, Action<string, object>>>();

        public static void ForEachProperty(object instance, Action<string, object> send)
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
                        foreach (DictionaryEntry kv in dict)
                            s((kv.Key ?? "").ToString(), kv.Value);
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