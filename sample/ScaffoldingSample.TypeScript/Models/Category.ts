import { Product } from './Product';

export interface Category {
    categoryId: number;
    categoryName: string;
    product: Product[];
}
