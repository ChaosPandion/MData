﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.SqlCe
{
    public class SqlCeDatabase : Database<System.Data.SqlServerCe.SqlCeConnection>
    {
        public SqlCeDatabase(string connectionString)
            : base(connectionString)
        {

        }

        protected override ICommand CreateCommand()
        {
            return new SqlCeCommand(this);
        }
    }
}
