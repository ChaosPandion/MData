using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
    public interface IReader : IDisposable
    {
        int FieldCount { get; }

        bool ReadResult();

        bool ReadRecord();

        IField GetField(int index);

        IField GetField(string name);

        T GetFieldValue<T>(int index);

        T GetFieldValue<T>(string name);

        string GetFieldName(int index);

        int GetFieldIndex(string name);

        IEnumerable<string> GetFieldNames();

        IEnumerable<IField> GetFields();
    }
}