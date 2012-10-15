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
    public sealed class SqlServerSource : Source<SqlConnection>
    {
        public SqlServerSource(string connectionString)
            : base(connectionString)
        {

        }

        public override IRequestBuilder CreateRequestBuilder()
        {
            return new SqlServerRequestBuilder(this);
        }
    }
}