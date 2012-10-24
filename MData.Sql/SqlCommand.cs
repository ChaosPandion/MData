using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.Sql
{
    public sealed class SqlCommand : Command<System.Data.SqlClient.SqlConnection>
    {
        public SqlCommand(Database<System.Data.SqlClient.SqlConnection> db)
            : base(db)
        {

        }
    }
}