using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface ICommand
    {
		dynamic Procedures { get; }

        ICommand SetText(string value);

        ICommand SetProcedure(string value);

        ICommand SetTimeout(int value);

        ICommand AddArgument<T>(string name, T value);

        ICommand AddArguments(IDictionary<string, object> args);

		ICommand AddArguments<T>(T value);

		void Execute();

		T Execute<T>();

		IRecord ExecuteRecord();

		IResult ExecuteResult();

        IResultCollection ExecuteResults();

		IReader ExecuteReader();

        T ExecuteEntity<T>()
            where T : new();

        T ExecuteEntity<T>(T entity);

        T ExecuteEntity<T>(Func<T> createInstance);

        IEnumerable<T> ExecuteEntityCollection<T>()
            where T : new();

        IEnumerable<T> ExecuteEntityCollection<T>(Func<T> createInstance);
    }
}