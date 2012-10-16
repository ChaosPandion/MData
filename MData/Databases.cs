using MData.Providers.SqlServer;
using MData.Providers.SqlServerCompact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public static class Databases
    {
        public static IDatabase GetSqlServerDatabase(string connectionString)
        {
            return new SqlServerDatabase(connectionString);
        }

		//public static ISource GetSqlServerCompactSource(string connectionString)
		//{
		//    return new SqlServerCompactSource(connectionString);
		//}
    }
}