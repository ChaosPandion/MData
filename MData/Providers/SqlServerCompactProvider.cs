using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;

namespace MData.Providers
{
    class SqlServerCompactProvider : IProvider
    {
        private readonly string _connectionString;

        public SqlServerCompactProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool SupportsChainedContext
        {
            get { return false; }
        }

        public IDbCommand CreateCommand(string text, object args = null)
        {
            var cn = new SqlCeConnection(_connectionString);
            var cm = cn.CreateCommand();
            cm.CommandText = text;
            cm.CommandType = CommandType.Text;
            if (args != null)
            {
                Reflection.ForEachProperty(args, (key, val) =>
                {
                    var cmp = cm.CreateParameter();
                    cmp.ParameterName = key;
                    cmp.Value = val;
                    cm.Parameters.Add(cmp);
                });
            }
            cn.Open();
            return cm;
        }

        public object GetReaderValue(IDataReader reader, int index)
        {
            throw new NotImplementedException();
        }

        public string FormatParameterName(string name)
        {
            throw new NotImplementedException();
        }

        public string FormatProcedureCall(string name, int argCount, Func<int, string> getArgName)
        {
            throw new NotImplementedException();
        }

        public string CombineObjectName(string left, string right)
        {
            if (!string.IsNullOrEmpty(left))
                throw new Exception();
            return right;
        }
    }
}
