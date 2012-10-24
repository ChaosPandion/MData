using System.Collections.Generic;
using System.Dynamic;

namespace MData
{
    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    public interface IRecord : IDynamicMetaObjectProvider, IEnumerable<IField>
    {
        int ResultCount { get; }

        int RecordCount { get; }


        int ResultIndex { get; }

        int RecordIndex { get; }


        IRecord NextResult { get; }

        IRecord NextRecord { get; }


		/// <summary>
		/// Gets the number of fields.
		/// </summary>
		int FieldCount { get; }

        /// <summary>
        /// Gets the value of the field at the specified index.
        /// </summary>
        /// <typeparam name="T">The expected type of the field.</typeparam>
        /// <param name="index">The index of the field.</param>
        T GetValue<T>(int index);

        /// <summary>
		/// Gets the value of the field with the specified name.
        /// </summary>
		/// <typeparam name="T">The expected type of the field.</typeparam>
        /// <param name="name">The name of the field.</param>
		T GetValue<T>(string name);

        /// <summary>
		/// Gets the field at the specified index.
        /// </summary>
		/// <param name="index">The index of the field.</param>
		IField GetField(int index);

        /// <summary>
		/// Gets the field with the specified name.
        /// </summary>
		/// <param name="name">The name of the field.</param>
		IField GetField(string name);
    }
}