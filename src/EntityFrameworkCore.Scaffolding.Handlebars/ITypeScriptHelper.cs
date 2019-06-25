using System;

namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// TypeScript Helper
    /// </summary>
    public interface ITypeScriptHelper
    {
        /// <summary>
        /// Convert CLR type to TypeScript type
        /// </summary>
        /// <param name="clrType">CLR type.</param>
        /// <returns>TypeScript type</returns>
        string TypeName(Type clrType);

        /// <summary>
        /// Convert string to camel case.
        /// </summary>
        /// <param name="s">Input string.</param>
        /// <returns>Input string in camel case.</returns>
        string ToCamelCase(string s);
    }
}
