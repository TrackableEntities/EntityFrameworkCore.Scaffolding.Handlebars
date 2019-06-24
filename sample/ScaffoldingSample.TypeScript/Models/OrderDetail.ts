import { Order } from './Order';
import { Product } from './Product';

export interface OrderDetail {
    orderDetailId: number;
    orderId: number;
    productId: number;
    unitPrice: number;
    quantity: number;
    discount: number;
    order: Order;
    product: Product;
}
