export interface Product {
    id?: number;
    name: string;
    price: number;
    description?: string;
    createdAt?: Date;
    updatedAt?: Date | null;
}

export interface CreateProductDto {
    name: string;
    price: number;
    description?: string;
}

export interface UpdateProductDto {
    id: number;
    name: string;
    price: number;
    description?: string;
}
