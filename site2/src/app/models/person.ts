export interface Person {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    address?: string;
    createdAt?: string;
    updatedAt?: string | null;
}

export interface CreatePersonDto {
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    address?: string;
}

export interface UpdatePersonDto extends Partial<CreatePersonDto> {}
