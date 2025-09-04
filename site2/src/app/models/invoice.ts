import { Client } from "./client";
import { Detail } from "./detail";

export class Invoice {

    id?: number;
    statusId: number;
    serial: string;
    number: number;
    clientId: number;
    client: Client;
    details: Detail[] = [];

}
