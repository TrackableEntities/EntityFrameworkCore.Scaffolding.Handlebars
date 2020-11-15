// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2020 Tony Sneed.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;

namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal
{
    [DebuggerStepThrough]
    internal static class SharedTypeExtensions
    {
        private static readonly Dictionary<Type, string> _builtInTypeNames = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
            { typeof(void), "void" }
        };

        public static bool IsNullableType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return !typeInfo.IsValueType
                   || typeInfo.IsGenericType
                   && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static IEnumerable<string> GetNamespaces([NotNull] this Type type)
        {
            if (_builtInTypeNames.ContainsKey(type))
            {
                yield break;
            }

            yield return type.Namespace;

            if (type.IsGenericType)
            {
                foreach (var typeArgument in type.GenericTypeArguments)
                {
                    foreach (var ns in typeArgument.GetNamespaces())
                    {
                        yield return ns;
                    }
                }
            }
        }
    }
}
