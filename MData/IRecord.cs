using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace MData
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRecord : IDynamicMetaObjectProvider, IEnumerable<IField>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        T GetValue<T>(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
		T GetValue<T>(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
		IField GetField(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
		IField GetField(string name);
    }
}