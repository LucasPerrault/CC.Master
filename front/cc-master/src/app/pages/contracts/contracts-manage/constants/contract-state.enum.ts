import { IContractState } from '../models/contract-state.interface';

export enum ContractState {
  NotStarted = 0,
  InProgress = 1,
  Closed = 2,
}

export const contractStates: IContractState[] = [
  {
    id: ContractState.NotStarted,
    name: 'front_contractPage_contractStates_notStarted',
  },
  {
    id: ContractState.InProgress,
    name: 'front_contractPage_contractStates_inProgress',
  },
  {
    id: ContractState.Closed,
    name: 'front_contractPage_contractStates_closed',
  },
];
