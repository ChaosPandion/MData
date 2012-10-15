using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using MData.Foundation;

namespace MData.Providers.SqlServerCompact
{
    public sealed class SqlServerCompactSource : Source<SqlCeConnection>
    {
        public SqlServerCompactSource(string connectionString)
            : base(connectionString)
        {

        }

        public override IRequestBuilder CreateRequestBuilder()
        {
            return new SqlServerCompactRequestBuilder(this);
        }
    }
}