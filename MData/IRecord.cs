using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface IRecord : IDynamicMetaObjectProvider, IEnumerable<IField>
    {
        T GetValue<T>(int index);
        T GetValue<T>(string name);
    }
}