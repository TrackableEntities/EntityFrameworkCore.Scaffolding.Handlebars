import { Category } from './Category';
import { OrderDetail } from './OrderDetail';

export class Product {

    ProductId: number;
    ProductName: string;
    CategoryId: number;
    UnitPrice: number;
    Discontinued: boolean;
    RowVersion: any;
    Category: Category;
    OrderDetail: OrderDetail[];

    constructor() {
    }
}
