using System;
using System.Dynamic;

namespace MData
{
    /// <summary>
    /// Represents a named value.
    /// </summary>
	public interface IField : IEquatable<IField>
    {
        /// <summary>
        /// Gets the Name of the current instance.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Type of the current instance.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the Value of the current instance.
        /// </summary>
        object Value { get; }
    }
}