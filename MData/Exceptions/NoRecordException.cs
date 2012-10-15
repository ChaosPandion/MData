using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData.Exceptions
{
    [Serializable]
    public sealed class NoRecordException : MException
    {
        public NoRecordException()
            : base("The result set contains no records.")
        {

        }
    }
}