import { Employee } from './Employee';
import { Territory } from './Territory';

export interface EmployeeTerritory {
    employeeId: number;
    territoryId: string;
    employee: Employee;
    territory: Territory;
}
