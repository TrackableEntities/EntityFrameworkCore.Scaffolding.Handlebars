import { CustomerSetting } from './CustomerSetting';
import { Order } from './Order';

export class Customer {

    CustomerId: string;
    CompanyName: string;
    ContactName: string;
    City: string;
    Country: string;
    CustomerSetting: CustomerSetting;
    Order: Order[];

    constructor() {
    }
}
