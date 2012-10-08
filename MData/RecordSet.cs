using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public sealed class RecordSet : IDisposable, IEnumerable<FieldSet>
    {
        private readonly List<FieldSet> _fieldSets = new List<FieldSet>();
        private bool _isFullyCached = false;
        private readonly ADO ADO;

        internal RecordSet(ADO ado)
        {
            ADO = ado;
        } 

        public void Dispose()
        {
            lock (ADO)
            {
                ADO.Dispose();
                _isFullyCached = true;
            }
        }

        public IEnumerator<FieldSet> GetEnumerator()
        {
            var fs = default(FieldSet);
            var index = 0;
            while (true)
            {
                if (_isFullyCached)
                {
                    for (; index < _fieldSets.Count; index++)
                        yield return _fieldSets[index];
                    yield break;
                }
                lock (ADO)
                {
                    if (_isFullyCached)
                        continue;
                    if (!ADO.Reader.Read())
                    {
                        _isFullyCached = true;
                        yield break;
                    }
                    fs = new FieldSet(ADO);
                    _fieldSets.Add(fs);
                    yield return fs;
                    index++;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
