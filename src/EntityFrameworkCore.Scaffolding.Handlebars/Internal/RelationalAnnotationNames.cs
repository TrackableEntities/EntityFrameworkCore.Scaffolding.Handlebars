namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal
{
    /// <summary>
    ///     Names for well-known relational model annotations. Applications should not use these names
    ///     directly, but should instead use the extension methods on metadata objects.
    /// </summary>
    public static class RelationalAnnotationNames2
    {
        /// <summary>
        ///     The prefix used for any relational annotation.
        /// </summary>
        public const string Prefix = "Relational:";

        /// <summary>
        ///     The definition of a database view.
        /// </summary>
        public const string ViewDefinition = Prefix + "ViewDefinition";
    }
}
