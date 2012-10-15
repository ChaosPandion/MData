using MData.Foundation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;

namespace MData.Providers.SqlServerCompact
{
    public sealed class SqlServerCompactRequestBuilder : RequestBuilder<SqlCeConnection>
    {
        public SqlServerCompactRequestBuilder(SqlServerCompactSource source)
            : base(source)
        {

        }
    }
}