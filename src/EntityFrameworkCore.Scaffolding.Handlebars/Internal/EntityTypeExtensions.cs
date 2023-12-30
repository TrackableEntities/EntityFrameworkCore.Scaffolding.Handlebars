using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EntityFrameworkCore.Scaffolding.Handlebars.Internal;

/// <summary>
///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
///     the same compatibility standards as public APIs. It may be changed or removed without notice in
///     any release. You should only use it directly in your code with extreme caution and knowing that
///     doing so can result in application failures when updating to a new Entity Framework Core release.
/// </summary>
public static class EntityTypeExtensions
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public static IEnumerable<IPropertyBase> GetPropertiesAndNavigations(
        this IEntityType entityType)
        => entityType.GetProperties().Concat<IPropertyBase>(entityType.GetNavigations());

    /// <summary>
    /// Determines if the given <see cref="IEntityType"/> is a join entity 
    /// type for a many-to-many relationship where Entity would not be generated.
    /// This is where only Key properties are present.
    /// </summary>
    /// <param name="entityType">Entity Type</param>
    /// <returns></returns>
    public static bool IsManyToManyJoinEntityType(this IEntityType entityType)
    {
        if (!entityType.GetNavigations().Any()
            && !entityType.GetSkipNavigations().Any())
        {
            var primaryKey = entityType.FindPrimaryKey();
            var properties = entityType.GetProperties().ToList();
            var foreignKeys = entityType.GetForeignKeys().ToList();
            if (primaryKey != null
                && primaryKey.Properties.Count > 1
                && foreignKeys.Count == 2
                && primaryKey.Properties.Count == properties.Count
                && foreignKeys[0].Properties.Count + foreignKeys[1].Properties.Count == properties.Count
                && !foreignKeys[0].Properties.Intersect(foreignKeys[1].Properties).Any()
                && foreignKeys[0].IsRequired
                && foreignKeys[1].IsRequired
                && !foreignKeys[0].IsUnique
                && !foreignKeys[1].IsUnique)
            {
                return true;
            }
        }
        return false;
    }
}