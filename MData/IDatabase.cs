using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface IDatabase
    {
        string ConnectionString { get; }
        ICommand BuildCommand(Func<ICommand, ICommand> configure);
        ICommand BuildTextCommand(string text, object args = null, Func<ICommand, ICommand> configure = null);
        ICommand BuildDynamicCommand(Func<dynamic, dynamic> configure);
    }
}
