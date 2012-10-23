using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MData
{
	static class RecordSetBinder<T>
	{
		private static readonly Func<IRecordSet, Func<T>, IEnumerable<T>> _binder = CreateBinder();

		public static IEnumerable<T> Bind(IRecordSet recordSet, Func<T> createInstance)
		{
			recordSet.ThrowIfNull("recordSet");
			createInstance.ThrowIfNull("createInstance");
			return _binder(recordSet, createInstance);
		}

		private static Func<IRecordSet, Func<T>, IEnumerable<T>> CreateBinder()
		{
			return (recordSet, createInstance) =>
			{
				var list = new List<T>();
				foreach (var record in recordSet)
					list.Add(RecordBinder<T>.Bind(record, createInstance));
				return list.AsReadOnly();
			};
		}
	}
}