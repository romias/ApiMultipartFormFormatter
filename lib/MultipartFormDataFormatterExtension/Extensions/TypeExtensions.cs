using System;
using System.Collections.Generic;

namespace MultipartFormDataFormatterExtension.Extensions
{
    internal static class TypeExtensions
    {
        #region Methods

        /// <summary>
        ///     Whether instance is a collection or not.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsList(this Type type)
        {
            if (!type.IsGenericType)
                return false;

            return type.GetInterface(typeof(IEnumerable<>).FullName) != null;
        }

        #endregion
    }
}