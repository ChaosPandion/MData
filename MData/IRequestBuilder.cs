using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface IRequestBuilder
    {
        IRequestBuilder SetText(string value);
        IRequestBuilder SetProcedure(string value);
        IRequestBuilder SetTimeout(int value);
        IRequestBuilder AddArgument<T>(string name, T value);
        IRequestBuilder AddArguments(IDictionary<string, object> args);
        IRequestBuilder AddArguments<T>(T value);

        void Request();
        T Request<T>();
        IRecord RequestRecord();
        IList<IRecord> RequestRecords();
        IList<IList<IRecord>> RequestResults();
        IReader RequestReader();
    }
}
