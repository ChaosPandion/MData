using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MData
{
	static class RecordBinder<T>
	{
		private static readonly Func<IRecord, T, T> _binder = CreateBinder();

		public static T Bind(IRecord record, Func<T> createInstance)
		{
			record.ThrowIfNull("record");
			createInstance.ThrowIfNull("createInstance");
			return _binder(record, createInstance());
		}

		private static Func<IRecord, T, T> CreateBinder()
		{
			return (record, instance) =>
			{
				foreach (var field in record)
				{
					var prop = typeof(T).GetProperty(field.Name);
					if (prop == null)
						continue;
					if (prop.PropertyType != field.Type)
						continue;
					prop.SetValue(instance, field.Value, null);
				}
				return instance;
			};
		}
	}
}