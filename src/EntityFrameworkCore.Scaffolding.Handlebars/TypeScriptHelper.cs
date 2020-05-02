using System;
using System.Globalization;
using System.Text;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// TypeScript Helper
    /// </summary>
    public class TypeScriptHelper : ITypeScriptHelper
    {
        /// <summary>
        /// Convert CLR type to TypeScript type
        /// </summary>
        /// <param name="clrType">CLR type.</param>
        /// <returns>TypeScript type</returns>
        public string TypeName(Type clrType)
        {
            var result = "any";
            if (clrType == typeof(bool) || clrType == typeof(bool?))
                result = "boolean";
            if (clrType == typeof(char) || clrType == typeof(char?)
                || clrType == typeof(string))
                result = "string";
            if (clrType == typeof(DateTime) || clrType == typeof(DateTime?))
                result = "Date";
            if (clrType == typeof(byte) || clrType == typeof(byte?)
                || clrType == typeof(sbyte) || clrType == typeof(sbyte?)
                || clrType == typeof(decimal) || clrType == typeof(decimal?)
                || clrType == typeof(double) || clrType == typeof(double?)
                || clrType == typeof(short) || clrType == typeof(short?)
                || clrType == typeof(ushort) || clrType == typeof(ushort?)
                || clrType == typeof(int) || clrType == typeof(int?)
                || clrType == typeof(uint) || clrType == typeof(uint?)
                || clrType == typeof(long) || clrType == typeof(long?)
                || clrType == typeof(ulong) || clrType == typeof(ulong?)
                || clrType == typeof(float) || clrType == typeof(float?))
                result = "number";
            return result;
        }

        /// <summary>
        /// Convert string to camel case.
        /// </summary>
        /// <param name="s">Input string.</param>
        /// <returns>Input string in camel case.</returns>
        public string ToCamelCase(string s)
        {
            if (s == null || s.Length < 2)
                return s;
            var chars = s.ToCharArray();
            var sb = new StringBuilder();
            sb.Append(chars[0].ToString().ToLower(CultureInfo.InvariantCulture));
            for (int i = 1; i < chars.Length; i++)
            {
                sb.Append(chars[i]);
            }
            return sb.ToString();
        }
    }
}
