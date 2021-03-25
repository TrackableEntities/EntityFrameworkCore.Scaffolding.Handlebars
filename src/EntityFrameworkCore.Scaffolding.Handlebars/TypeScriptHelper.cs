using System;
using System.Globalization;
using System.Text;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// TypeScript Helper
    /// </summary>
    public class TypeScriptHelper 
    {
        /// <summary>
        /// Convert CLR type to TypeScript type
        /// </summary>
        /// <param name="clrType">CLR type.</param>
        /// <returns>TypeScript type</returns>
        public static string TypeName(string clrType)
        {
            var result = "any";
            if (clrType =="bool" || clrType == "bool?")
                result = "boolean";
            if (clrType == "char" || clrType == "char?"
                || clrType == "string")
                result = "string";
            if (clrType == "DateTime" || clrType == "DateTime?")
                result = "Date";
            if (clrType == "byte" || clrType == "byte?"
                || clrType == "sbyte" || clrType == "sbyte?"
                || clrType == "decimal" || clrType == "decimal?"
                || clrType == "double" || clrType == "double?"
                || clrType == "short" || clrType == "short?"
                || clrType == "ushort" || clrType == "ushort?"
                || clrType == "int" || clrType == "int?"
                || clrType == "uint" || clrType == "uint?"
                || clrType == "long" || clrType == "long?"
                || clrType == "ulong" || clrType == "ulong?"
                || clrType == "float" || clrType == "float?")
                result = "number";
            return result;
        }

        /// <summary>
        /// Convert string to camel case.
        /// </summary>
        /// <param name="s">Input string.</param>
        /// <returns>Input string in camel case.</returns>
        public static string ToCamelCase(string s)
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
