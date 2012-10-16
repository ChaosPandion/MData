using MData.Foundation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MData.Providers.SqlServer
{
    public sealed class SqlServerDatabase : Database<SqlConnection>
    {
        public SqlServerDatabase(string connectionString)
            : base(connectionString)
        {

        }

        protected override ICommand CreateCommandBuilder()
        {
            return new SqlServerCommand(this);
        }
    }
}