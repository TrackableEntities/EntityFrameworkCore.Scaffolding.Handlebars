import { CustomerSetting } from './CustomerSetting';
import { Order } from './Order';

export interface Customer {
    customerId: string;
    companyName: string;
    contactName: string;
    city: string;
    country: string;
    customerSetting: CustomerSetting;
    orders: Order[];
}
