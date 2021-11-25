import { Employee } from './Employee';

export interface Territory {
    territoryId: string;
    territoryDescription: string;
    employees: Employee[];
}
