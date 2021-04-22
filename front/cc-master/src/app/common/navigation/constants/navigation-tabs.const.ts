import { Operation, OperationRestrictionMode } from '@cc/aspects/rights';

import { INavigationTab } from '../models/navigation-tab.interface';
import { NavigationAlert } from './navigation-alert.enum';
import { NavigationPath } from './navigation-path.enum';

export const navigationTabs: INavigationTab[] = [
    {
      name: 'front_navigation_environmentsTab',
      url: NavigationPath.Environments,
      icon: 'establishment',
      isLegacy: true,
      restriction: { operations: [Operation.DisplayEnvTab] },
    },
    {
      name: 'front_navigation_sourcesTab',
      url: NavigationPath.CodeSources,
      icon: 'branch',
      isLegacy: true,
      restriction: { operations: [Operation.ReadCodeSources] },
    },
    {
      name: 'front_navigation_ftpTab',
      url: NavigationPath.Ftp,
      icon: 'upload',
      isLegacy: true,
      restriction: { operations: [Operation.ReadFtp] },
    },
    {
      name: 'front_navigation_demosTab',
      url: NavigationPath.Demos,
      icon: 'lucca',
      isLegacy: true,
      restriction: { operations: [Operation.Demo] },
    },
    {
      name: 'front_navigation_contractsTab',
      url: NavigationPath.Contracts,
      icon: 'file',
      restriction: { operations: [Operation.ReadContracts] },
      children: [
        {
          url: NavigationPath.ContractsDraft,
          name: 'front_navigation_draftsTab',
          isLegacy: true,
        },
        {
          url: NavigationPath.ContractsManage,
          name: 'front_navigation_contractManageTab',
          isLegacy: true,
        },
      ],
    },
    {
      name: 'front_navigation_countsTab',
      url: NavigationPath.Counts,
      icon: 'analytics',
      isLegacy: true,
      restriction: { operations: [Operation.CountsTrack] },
    },
    {
      name: 'front_navigation_accountingTab',
      url: NavigationPath.Accounting,
      icon: 'book',
      restriction: { operations: [Operation.CreateCounts] },
      children: [
        {
          url: NavigationPath.AccountingRevenue,
          name: 'front_navigation_accountingRevenueTab',
          isLegacy: true,
        },
        {
          url: NavigationPath.AccountingMiscTransactions,
          name: 'front_navigation_accountingMiscTransactionsTab',
          isLegacy: true,
        },
      ],
    },
    {
      name: 'front_navigation_billingTab',
      url: NavigationPath.Billing,
      icon: 'bill',
      restriction: {
        operations: [Operation.Invoices, Operation.ExportClients],
        mode: OperationRestrictionMode.Some,
      },
      children: [
        {
          url: NavigationPath.BillingSynthesis,
          name: 'front_navigation_billingSynthesisTab',
          isLegacy: true,
        },
        {
          url: NavigationPath.BillingToDo,
          name: 'front_navigation_billingToDoTab',
          isLegacy: true,
        },
        {
          url: NavigationPath.BillingToExport,
          name: 'front_navigation_billingExportTab',
          alert: NavigationAlert.BillingToExport,
          isLegacy: true,
        },
        {
          url: NavigationPath.BillingExported,
          name: 'front_navigation_billingExportedTab',
          isLegacy: true,
        },
      ],
    },
    {
      name: 'front_navigation_cmrrTab',
      url: NavigationPath.Cmrr,
      icon: 'evolution',
      restriction: { operations: [Operation.ReadCMRR] },
      isLegacy: true,
    },
    {
      name: 'front_navigation_distributorsTab',
      url: NavigationPath.Distributors,
      icon: 'userGroup',
      restriction: {
        operations: [
          Operation.ReadDistributorsTab,
          Operation.EditDistributors,
          Operation.EnvironmentAccessesAndDistributorRelations,
          Operation.ReadDistributorRelations,
        ],
        mode: OperationRestrictionMode.Some,
      },
      children: [
        {
          url: NavigationPath.DistributorsManage,
          name: 'front_navigation_distributorsManageTab',
          isLegacy: true,
        },
        {
          url: NavigationPath.DistributorsEnvironmentAccesses,
          name: 'front_navigation_distributorsEnvironmentAccessesTab',
          isLegacy: true,
        },
      ],
    },
    {
      name: 'front_navigation_offersTab',
      url: NavigationPath.Offers,
      icon: 'pricetag',
      restriction: { operations: [Operation.ReadCommercialOffers] },
      isLegacy: true,
    },
    {
      name: 'front_navigation_logsTab',
      url: NavigationPath.Logs,
      icon: 'list',
      restriction: { operations: [Operation.EnvironmentLogTab] },
    },
  ];
