import { EmployeeTerritories } from './EmployeeTerritories';

export interface Employee {
    employeeId: number;
    lastName: string;
    firstName: string;
    birthDate: Date;
    hireDate: Date;
    city: string;
    country: string;
    employeeTerritories: EmployeeTerritories[];
}
