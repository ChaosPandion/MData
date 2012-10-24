using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface IReader : IDisposable, IFieldMap
    {
        int ResultIndex { get; }
        int RecordIndex { get; }

        bool ReadResult();
        bool ReadRecord();
    }
}