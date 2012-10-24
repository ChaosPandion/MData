using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MData.SqlCe
{
    public sealed class SqlCeCommand : Command<System.Data.SqlServerCe.SqlCeConnection>
    {
        public SqlCeCommand(Database<System.Data.SqlServerCe.SqlCeConnection> db)
            : base(db)
        {

        }
    }
}