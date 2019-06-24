import { Order } from './Order';
import { Product } from './Product';

export class OrderDetail {

    OrderDetailId: number;
    OrderId: number;
    ProductId: number;
    UnitPrice: number;
    Quantity: number;
    Discount: number;
    Order: Order;
    Product: Product;

    constructor() {
    }
}
