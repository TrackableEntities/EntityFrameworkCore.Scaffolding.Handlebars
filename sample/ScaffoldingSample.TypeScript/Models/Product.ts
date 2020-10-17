import { Category } from './Category';
import { OrderDetail } from './OrderDetail';

export interface Product {
    productId: number;
    productName: string;
    categoryId: number;
    unitPrice: number;
    discontinued: boolean;
    rowVersion: any;
    category: Category;
    orderDetails: OrderDetail[];
}
