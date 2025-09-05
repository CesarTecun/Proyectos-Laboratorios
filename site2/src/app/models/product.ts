export interface Product {
    id?: number;
    name: string;
    price: number;
    createdAt?: Date;
    updatedAt?: Date | null;
}

export interface CreateProductDto {
    name: string;
    price: number;
}

export interface UpdateProductDto {
    id: number;
    name: string;
    price: number;
}
