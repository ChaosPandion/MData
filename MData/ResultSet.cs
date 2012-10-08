using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    public sealed class ResultSet : IDisposable, IEnumerable<RecordSet>
    {
        private readonly List<RecordSet> _recordSets = new List<RecordSet>();
        private readonly ADO ADO;
        private bool _isFullyCached = false;

        internal ResultSet(ADO ado)
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

        public IEnumerator<RecordSet> GetEnumerator()
        {
            var rs = default(RecordSet);
            var index = 0;
            while (true)
            {
                if (_isFullyCached)
                {
                    for (; index < _recordSets.Count; index++)
                        yield return _recordSets[index];
                    yield break;
                }
                lock (ADO)
                {
                    if (_isFullyCached)
                        continue;
                    if (index > 0 && !ADO.Reader.NextResult())
                    {
                        ADO.Dispose();
                        _isFullyCached = true;
                        yield break;
                    }
                    rs = new RecordSet(ADO);
                    _recordSets.Add(rs);
                    yield return rs;
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
