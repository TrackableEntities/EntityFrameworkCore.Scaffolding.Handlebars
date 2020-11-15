import { EmployeeTerritory } from './EmployeeTerritory';

export interface Territory {
    territoryId: string;
    territoryDescription: string;
    employeeTerritories: EmployeeTerritory[];
}
