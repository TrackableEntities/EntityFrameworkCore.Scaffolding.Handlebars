import { Employee } from './Employee';
import { Territory } from './Territory';

export interface EmployeeTerritories {
    employeeId: number;
    territoryId: string;
    employee: Employee;
    territory: Territory;
}
