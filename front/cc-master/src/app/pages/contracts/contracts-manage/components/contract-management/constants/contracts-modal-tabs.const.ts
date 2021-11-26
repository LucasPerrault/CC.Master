import { Operation } from '@cc/aspects/rights';
import { INavigationTab } from '@cc/common/navigation';

import { ContractsModalTabPath } from './contracts-modal-tab-path.enum';

export const contractsModalTabs: INavigationTab[] = [
  {
    name: 'front_contractPage_contractTab',
    url: ContractsModalTabPath.Contract,
    restriction: { operations: [Operation.ReadContracts] },
  },
  {
    name: 'front_contractPage_countTab',
    url: ContractsModalTabPath.Counts,
    restriction: { operations: [Operation.ReadCounts] },
  },
  {
    name: 'front_contractPage_accountingTab',
    url: ContractsModalTabPath.Accounting,
    restriction: { operations: [Operation.ReadContractEntries] },
  },
  {
    name: 'front_contractPage_environmentTab',
    url: ContractsModalTabPath.Environment,
    restriction: { operations: [Operation.ReadContracts] },
  },
  {
    name: 'front_contractPage_offerTab',
    url: ContractsModalTabPath.Offer,
    restriction: { operations: [Operation.ReadContracts, Operation.CloseContracts] },
  },
  {
    name: 'front_contractPage_establishmentsTab',
    url: ContractsModalTabPath.Establishments,
    restriction: { operations: [Operation.ReadContracts] },
  },
  {
    name: 'front_contractPage_closeTab',
    url: ContractsModalTabPath.Close,
    restriction: { operations: [Operation.CloseContracts] },
  },
  {
    name: 'front_contractPage_historyTab',
    url: ContractsModalTabPath.History,
    restriction: { operations: [Operation.ReadContracts] },
  },
];

