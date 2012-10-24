using System;
using System.Collections.Generic;

namespace MData
{
    /// <summary>
    /// Represents an object that contains instances of <see cref="MData.IField" /> which are accessible by name or index.
    /// </summary>
    public interface IFieldMap : IEnumerable<IField>
    {		
        /// <summary>
        /// Gets the number of fields.
        /// </summary>
        int FieldCount { get; }

        /// <summary>
        /// Gets a collection containing the field names.
        /// </summary>
        IEnumerable<string> GetFieldNames();

        /// <summary>
        /// Returns the name of the field with the specified <paramref name="index" />. 
        /// </summary>
        /// <param name="index">The index of the field.</param>
        string GetName(int index);

        /// <summary>
        /// Returns the type of the field with the specified <paramref name="index" />. 
        /// </summary>
        /// <param name="index">The index of the field.</param>
        Type GetType(int index);

        /// <summary>
        /// Returns the type of the field with the specified name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        Type GetType(string name);

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