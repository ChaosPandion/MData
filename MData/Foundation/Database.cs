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
using MData.Support;

namespace MData.Foundation
{
	public abstract class Database<TConnection> : IDatabase
		where TConnection : IDbConnection, new()
	{        
        private readonly string _connectionString;

        protected Database(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public ICommandBuilder BuildCommand(Func<ICommandBuilder, ICommandBuilder> configure)
        {
			configure.ThrowIfNull("configure");
            var cb = CreateCommandBuilder();
			if (cb == null)
                throw new Exception();
			cb = configure(cb);
			if (cb == null)
                throw new Exception();
			return cb;
        }

        public abstract ICommandBuilder CreateCommandBuilder();
    }
}