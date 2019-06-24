import { EmployeeTerritories } from './EmployeeTerritories';

export class Employee {

    EmployeeId: number;
    LastName: string;
    FirstName: string;
    BirthDate: Date;
    HireDate: Date;
    City: string;
    Country: string;
    EmployeeTerritories: EmployeeTerritories[];

    constructor() {
    }
}
