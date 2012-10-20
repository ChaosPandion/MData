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

        object GetFieldValue(int index);

        T GetFieldValue<T>(int index);

        object GetFieldValue(string name);

        T GetFieldValue<T>(string name);

        string GetFieldName(int index);

        Type GetFieldType(int index);

        int GetFieldIndex(string name);

        IEnumerable<string> GetFieldNames();

        IEnumerable<IField> GetFields();
    }
}