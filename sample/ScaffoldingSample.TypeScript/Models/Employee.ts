import { Territory } from './Territory';

export interface Employee {
    employeeId: number;
    lastName: string;
    firstName: string;
    birthDate: Date;
    hireDate: Date;
    city: string;
    country: string;
    territories: Territory[];
}
