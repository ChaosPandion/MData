using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData
{
    internal sealed class ADO : IDisposable
    {
        public IDbConnection Connection { get; set; }
        public IDbCommand Command { get; set; }
        public IDataReader Reader { get; set; }

        public void Dispose()
        {
            if (Reader != null)
                Reader.Dispose();
            if (Command != null)
                Command.Dispose();
            if (Connection != null)
                Connection.Dispose();
        }
    }
}