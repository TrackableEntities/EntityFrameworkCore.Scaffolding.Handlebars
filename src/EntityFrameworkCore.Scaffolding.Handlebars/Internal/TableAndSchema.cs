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
