using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
	public interface IField : IEquatable<IField>
    {
        string Name { get; }
        Type Type { get; }
        object Value { get; }
    }
}
