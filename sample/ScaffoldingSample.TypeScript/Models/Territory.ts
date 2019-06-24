import { EmployeeTerritories } from './EmployeeTerritories';

export interface Territory {
    territoryId: string;
    territoryDescription: string;
    employeeTerritories: EmployeeTerritories[];
}
