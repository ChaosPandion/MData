using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MData.Providers
{
    interface IProvider
    {
        IDbCommand CreateCommand(string text, object args = null);

        object GetReaderValue(IDataReader reader, int index);

        string FormatParameterName(string name);

        string FormatProcedureCall(string name, int argCount, Func<int, string> getArgName);

        string CombineObjectName(string left, string right);
    }
}