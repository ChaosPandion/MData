using MData.Providers.SqlServer;
using MData.Providers.SqlServerCompact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public static class Sources
    {
        public static ISource GetSqlServerSource(string connectionString)
        {
            return new SqlServerSource(connectionString);
        }

        public static ISource GetSqlServerCompactSource(string connectionString)
        {
            return new SqlServerCompactSource(connectionString);
        }
    }
}