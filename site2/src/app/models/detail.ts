import { Invoice } from "./invoice";
import { Product } from "./product";

export class Detail {

    id?: number;
    statusId: number;
    price: number;
    total: number;
    invoiceId: number;
    invoice: Invoice;
    productId: number;
    product: Product;

}
