import { IContractLogType } from '../models/contract-log-type.interface';

export enum ContractLogType {
  Creation = 'Creation',
  Update = 'Update',
  Deletion = 'Deletion'
}

export const contractLogTypes: IContractLogType[] = [
  {
    id: ContractLogType.Creation,
    name: 'front_contractPage_contractLogType_creation',
  },
  {
    id: ContractLogType.Update,
    name: 'front_contractPage_contractLogType_modification',
  },
  {
    id: ContractLogType.Deletion,
    name: 'front_contractPage_contractLogType_deletion',
  },
];
