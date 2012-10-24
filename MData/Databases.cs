using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MData
{
    public static class Databases
    {
        public static IDatabase GetSqlCeDatabase(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            var appDomain = AppDomain.CurrentDomain;
            var assembly = appDomain.Load("MData.SqlCe");
            var type = assembly.GetType("MData.SqlCe.SqlCeDatabase", true);
            return (IDatabase)Activator.CreateInstance(type, connectionString);
        }

        public static IDatabase GetSQLiteDatabase(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            var appDomain = AppDomain.CurrentDomain;
            var assembly = appDomain.Load("MData.SQLite");
            var type = assembly.GetType("MData.SQLite.SQLiteDatabase", true);
            return (IDatabase)Activator.CreateInstance(type, connectionString);
        }
    }
}