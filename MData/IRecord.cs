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
		Option<T> TryGetValue<T>(int index);
		Option<T> TryGetValue<T>(string name);
		IField GetField(int index);
		IField GetField(string name);
		Option<IField> TryGetField(int index);
		Option<IField> TryGetField(string name);
    }
}