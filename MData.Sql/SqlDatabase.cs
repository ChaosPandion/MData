using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.Sql
{
    public class SqlDatabase : Database<System.Data.SqlClient.SqlConnection>
    {
        public SqlDatabase(string connectionString)
            : base(connectionString)
        {

        }

        protected override ICommand CreateCommand()
        {
            return new SqlCommand(this);
        }
    }
}
