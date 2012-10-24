using System;
using System.Collections.Generic;
using System.Dynamic;

namespace MData
{
    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    public interface IRecord : IDynamicMetaObjectProvider, IFieldMap
    {
        /// <summary>
        /// Gets the number of results within the result set.
        /// </summary>
        int ResultCount { get; }

        /// <summary>
        /// Gets the number of records within the record set.
        /// </summary>
        int RecordCount { get; }

        /// <summary>
        /// Gets the index of the result within the result set.
        /// </summary>
        int ResultIndex { get; }

        /// <summary>
        /// Gets the index of the record within the record set.
        /// </summary>
        int RecordIndex { get; }
        
        /// <summary>
        /// Gets the next result in the result set or null if there are no more results.
        /// </summary>
        IRecord NextResult { get; }

        /// <summary>
        /// Gets the next record in the record set or null if there are no more records.
        /// </summary>
        IRecord NextRecord { get; }


    }
}