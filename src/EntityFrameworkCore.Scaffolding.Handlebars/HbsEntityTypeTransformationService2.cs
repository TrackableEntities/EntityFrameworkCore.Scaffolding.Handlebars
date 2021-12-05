using Microsoft.EntityFrameworkCore.Metadata;
using System;
namespace EntityFrameworkCore.Scaffolding.Handlebars
{
    /// <summary>
    /// Default service for transforming entity type definitions.
    /// </summary>
    public class HbsEntityTypeTransformationService2 : HbsEntityTypeTransformationServiceBase
    {
        /// <summary>
        /// Entity name transformer.
        /// </summary>
        public new Func<string, string> EntityTypeNameTransformer { get => base.EntityTypeNameTransformer; }

        /// <summary>
        /// Entity file name transformer.
        /// </summary>
        public new Func<string, string> EntityFileNameTransformer { get => base.EntityFileNameTransformer; }

        /// <summary>
        /// Constructor transformer.
        /// </summary>
        public new Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> ConstructorTransformer { get => ConstructorTransformer2; }

        /// <summary>
        /// Property name transformer.
        /// </summary>
        public new Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> PropertyTransformer { get => PropertyTransformer2; }

        /// <summary>
        /// Navigation property name transformer.
        /// </summary>
        public new Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> NavPropertyTransformer { get => NavPropertyTransformer2; }

        /// <summary>
        /// HbsEntityTypeTransformationService constructor.
        /// </summary>
        /// <param name="entityTypeNameTransformer">Entity type name transformer.</param>
        /// <param name="entityFileNameTransformer">Entity file name transformer.</param>
        /// <param name="constructorTransformer">Constructor transformer.</param>
        /// <param name="propertyTransformer">Property name transformer.</param>
        /// <param name="navPropertyTransformer">Navigation property name transformer.</param>
        public HbsEntityTypeTransformationService2(
            Func<string, string> entityTypeNameTransformer = null,
            Func<string, string> entityFileNameTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> constructorTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> propertyTransformer = null,
            Func<IEntityType, EntityPropertyInfo, EntityPropertyInfo> navPropertyTransformer = null)
                : base(entityTypeNameTransformer, entityFileNameTransformer)
        {
            ConstructorTransformer2 = constructorTransformer;
            PropertyTransformer2 = propertyTransformer;
            NavPropertyTransformer2 = navPropertyTransformer;
        }
    }
}