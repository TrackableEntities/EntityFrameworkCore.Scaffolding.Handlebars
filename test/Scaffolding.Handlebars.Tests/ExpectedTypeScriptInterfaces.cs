namespace Scaffolding.Handlebars.Tests
{
    public partial class HbsTypeScriptScaffoldingGeneratorTests
    {
        private static class ExpectedEntities
        {
            public const string CategoryClass =
@"import { Product } from './Product';

/**
* A category of products
*/
export interface Category {
    categoryId: number;
    /**
    * The name of a category
    */
    categoryName: string;
    products: Product[];
}
";

            public const string ProductClass =
@"import { Category } from './Category';

export interface Product {
    productId: number;
    productName: string;
    unitPrice: number;
    discontinued: boolean;
    rowVersion: any;
    categoryId: number;
    category: Category;
}
";
        }
    }
}