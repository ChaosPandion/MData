using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface IDatabase
    {
        string ConnectionString { get; }
        ICommandBuilder BuildCommand(Func<ICommandBuilder, ICommandBuilder> configure);
        ICommandBuilder CreateCommandBuilder();
    }
}
