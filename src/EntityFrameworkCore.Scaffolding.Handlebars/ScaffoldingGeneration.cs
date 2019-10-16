// ReSharper disable once CheckNamespace
namespace Microsoft.EntityFrameworkCore.Design
{
    /// <summary>
    /// Language options for reverse engineering classes from an existing database.
    /// </summary>
    public enum ScaffoldingGeneration
    {
        /// <summary>
        /// Generate DbContext class only.
        /// </summary>
        DbContextOnly,

        /// <summary>
        /// Generate entity type classes only.
        /// </summary>
        EntitiesOnly,

        /// <summary>
        /// Generate both DbContext and entity type classes.
        /// </summary>
        DbContextAndEntities
    }
}