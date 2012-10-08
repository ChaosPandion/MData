using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public sealed class FieldSet : IEnumerable<Field>
    {
        private readonly Dictionary<string, Field> _map = new Dictionary<string, Field>();
        private readonly List<Field> _list = new List<Field>();

        internal FieldSet(ADO ado)
        {
            lock (ado)
            {
                for (int i = 0; i < ado.Reader.FieldCount; i++)
                {
                    var field = new Field(ado.Reader.GetName(i), ado.Reader.GetFieldType(i), ado.Reader.GetValue(i));
                    _list.Add(field);
                    _map.Add(field.Name, field);
                }
            }
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public T Get<T>(int index)
        {
            return (T)_list[index].Value;
        }

        public T Get<T>(string name)
        {
            return (T)_map[name].Value;
        }

        public IEnumerator<Field> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
