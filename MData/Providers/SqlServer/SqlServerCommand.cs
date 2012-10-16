using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MData.Foundation;
using System.Data.SqlClient;
using System.Data;

namespace MData.Providers.SqlServer
{
    public sealed class SqlServerCommand : Command<SqlConnection>
    {
        public SqlServerCommand(SqlServerDatabase source)
            : base(source)
        {

        }
    }
}