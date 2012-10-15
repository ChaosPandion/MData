using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface ISource
    {
        string ConnectionString { get; }
        IRequestBuilder BuildRequest(Func<IRequestBuilder, IRequestBuilder> config = null, Func<dynamic, dynamic> dynamicConfig = null);
        IRequestBuilder BuildProcedureRequest(string procedure, object args = null, Func<IRequestBuilder, IRequestBuilder> config = null);
        IRequestBuilder BuildTextRequest(string text, object args = null, Func<IRequestBuilder, IRequestBuilder> config = null);
        IRequestBuilder CreateRequestBuilder();
    }
}
