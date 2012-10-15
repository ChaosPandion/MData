﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface ICommandBuilder
    {
		dynamic Procedures { get; }

        ICommandBuilder SetText(string value);

        ICommandBuilder SetProcedure(string value);

        ICommandBuilder SetTimeout(int value);

        ICommandBuilder AddArgument<T>(string name, T value);

        ICommandBuilder AddArguments(IDictionary<string, object> args);

		ICommandBuilder AddArguments<T>(T value);

		void Execute();

		T Execute<T>();

		IRecord ExecuteRecord();

		IRecordSet ExecuteRecords();

		IResultSet ExecuteResults();

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