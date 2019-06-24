import { Customer } from './Customer';
import { OrderDetail } from './OrderDetail';

export class Order {

    OrderId: number;
    CustomerId: string;
    OrderDate: Date;
    ShippedDate: Date;
    ShipVia: number;
    Freight: number;
    Customer: Customer;
    OrderDetail: OrderDetail[];

    constructor() {
    }
}
