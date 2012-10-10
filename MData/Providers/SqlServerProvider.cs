using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MData.Providers
{
    class SqlServerProvider : IProvider
    {
        private static readonly Regex _spRegex = new Regex(@"^\s*(?:\[?[a-zA-Z][a-zA-Z0-9]*\]?\.)?\[?[a-zA-Z][a-zA-Z0-9]*\]?\s*$", RegexOptions.Compiled);

        private readonly string _connectionString;
        private readonly int _timeout;

        public SqlServerProvider(string connectionString, int timeout)
        {
            _connectionString = connectionString;
            _timeout = timeout;
        }


        public IDbCommand CreateCommand(string text, object args = null)
        {
            var cn = new SqlConnection(_connectionString);
            var cm = cn.CreateCommand();
            if (_timeout > 0)
                cm.CommandTimeout = _timeout;
            cm.CommandText = text;
            cm.CommandType = _spRegex.IsMatch(text) ? 
                CommandType.StoredProcedure : 
                CommandType.Text;
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
            if (reader.IsDBNull(index))
            {
                return null;
            }
            return reader.GetValue(index);
        }

        public string FormatParameterName(string name)
        {
            return "@" + (name ?? "").Trim();
        }

        public string FormatProcedureCall(string name, int argCount, Func<int, string> getArgName)
        {
            return "Execute " + name + " " + string.Join(",", Enumerable.Range(0, argCount).Select(getArgName).Select((n, i) => "@" + (n ?? "arg" + i) + (n != null ? "=@" + n : "")));
        }

        public string CombineObjectName(string left, string right)
        {
            left = left ?? "";
            right = right ?? "";
            if (string.IsNullOrWhiteSpace(left))
                return right;
            return left + "." + right;
        }
    }
}
