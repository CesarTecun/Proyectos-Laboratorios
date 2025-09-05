export interface Person {
    id: number;
    name: string;
    email: string;
    phone: string;
    address: string;
}

export type CreatePersonDto = Omit<Person, 'id'>;
export type UpdatePersonDto = Partial<CreatePersonDto>;
