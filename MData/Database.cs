using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data.Common;

namespace MData
{
    public class Database<TConnection> : IDatabase
		where TConnection : IDbConnection, new()
	{        
        private readonly string _connectionString;

        public Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public ICommand BuildCommand(Func<ICommand, ICommand> configure)
        {
            configure.ThrowIfNull("configure");
            var cb = configure(GetCommand());
            if (cb == null)
                throw new Exception();
            return cb;
        }

        public ICommand BuildDynamicCommand(Func<dynamic, dynamic> configure)
        {
            configure.ThrowIfNull("configure");
            var cb = configure(GetCommand());
            if (cb == null)
                throw new Exception();
            return cb;
        }

        protected virtual ICommand CreateCommand()
        {
            return new Command<TConnection>(this);
        }

        private ICommand GetCommand()
        {
            var cb = CreateCommand();
            if (cb == null)
                throw new Exception();
            return cb;
        }
    }
}