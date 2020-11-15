import { Product } from './Product';

/**
* hello table Customer
*/
export interface Category {
    categoryId: number;
    /**
    * hello CompanyName
    */
    categoryName: string;
    products: Product[];
}
