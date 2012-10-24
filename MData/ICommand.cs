using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface ICommand
    {
        ICommand WithText(string value);
        ICommand WithProcedure(string value);
        ICommand WithTimeout(int value);
        ICommand WithParam<T>(string name, T value);
        ICommand WithParams(IDictionary<string, object> args);
        ICommand WithParams<T>(T value);

		void Execute();
        T Execute<T>();
        IRecord ExecuteResults();
		IReader ExecuteReader();
    }
}