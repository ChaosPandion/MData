using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace MData.Oracle
{
    public class OracleDatabase : Database<OracleConnection>
    {
        public OracleDatabase(string connectionString)
            : base(connectionString)
        {

        }

        protected override ICommand CreateCommand()
        {
            return new OracleCommand(this);
        }
    }
}