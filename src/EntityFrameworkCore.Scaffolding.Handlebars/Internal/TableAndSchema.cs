// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

// Modifications copyright(C) 2018 Tony Sneed.

namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal
{
    internal class TableAndSchema
    {
        internal TableAndSchema(string tableName)
        {
            var parts = tableName.Split('.');
            if (parts.Length > 1)
            {
                Schema = parts[0];
                Table = parts[1];
            }
            else
            {
                Table = parts[0];
            }
        }

        internal string Schema { get; set; }

        internal string Table { get; set; }
    }
}
