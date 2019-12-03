﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MultipartFormDataFormatterExtension.Extensions
{
    internal static class TextExtensions
    {
        /// <summary>
        ///     Check whether text is only numeric or not.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string text)
        {
            var regexNumeric = new Regex("^[0-9]*$");
            return regexNumeric.IsMatch(text);
        }

        /// <summary>
        ///     Find content disposition parameters
        /// </summary>
        public static List<string> ToContentDispositionParameters(this string contentDispositionName,
            FindContentDispositionParametersHandler interceptor)
        {
            if (interceptor == null)
                return contentDispositionName.Replace("[", ",")
                    .Replace("]", ",")
                    .Replace(".", ",")
                    .Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToList();

            return interceptor(contentDispositionName);
        }

        public static object ToEnum(this string text, Type enumType)
        {
            if (int.TryParse(text, out var num))
                return Enum.ToObject(enumType, num);

            return Enum.Parse(enumType, text, true);
        }
    }
}