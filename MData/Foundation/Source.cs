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

namespace MData.Foundation
{
	public abstract class Source<TConnection> : ISource
        where TConnection : IDbConnection, new()
	{        
        private readonly string _connectionString;

        protected Source(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public IRequestBuilder BuildRequest(Func<IRequestBuilder, IRequestBuilder> config = null, Func<dynamic, dynamic> dynamicConfig = null)
        {
            if (config == null && dynamicConfig == null)
                throw new Exception();
            var rb = CreateRequestBuilder();
            if (rb == null)
                throw new Exception();
            if (config != null)
                rb = config(rb);
            if (dynamicConfig != null)
                rb = dynamicConfig(rb);
            if (rb == null)
                throw new Exception();
            return rb;
        }

        public IRequestBuilder BuildProcedureRequest(string procedureName, object args = null, Func<IRequestBuilder, IRequestBuilder> config = null)
        {
            if (config == null)
                return BuildRequest(rb => rb.SetProcedure(procedureName).AddArguments(args));
            return BuildRequest(rb => config(rb).SetProcedure(procedureName).AddArguments(args));
        }

        public IRequestBuilder BuildTextRequest(string text, object args = null, Func<IRequestBuilder, IRequestBuilder> config = null)
        {
            if (config == null)
                return BuildRequest(rb => rb.SetText(text).AddArguments(args));
            return BuildRequest(rb => config(rb).SetText(text).AddArguments(args));
        }

        public abstract IRequestBuilder CreateRequestBuilder();
    }
}