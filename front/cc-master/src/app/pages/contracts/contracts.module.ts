import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Operation, OperationsGuard } from '@cc/aspects/rights';
import { NavigationPath } from '@cc/common/navigation';

import { ContractsComponent } from './contracts.component';
import { ContractsDraftEntryModalComponent } from './contracts-draft/components/contracts-draft-modal/contracts-draft-modal.component';
import { ContractsDraftComponent } from './contracts-draft/contracts-draft.component';
import { ContractsDraftModule } from './contracts-draft/contracts-draft.module';
import { ContractsModalTabPath } from './contracts-manage/components/contract-management/constants/contracts-modal-tab-path.enum';
import { ContractManagementComponent } from './contracts-manage/components/contract-management/contract-management.component';
import { AccountingTabComponent } from './contracts-manage/components/contract-management/tabs/accounting-tab/accounting-tab.component';
import { CloseTabComponent } from './contracts-manage/components/contract-management/tabs/close-tab/close-tab.component';
import { ContractTabComponent } from './contracts-manage/components/contract-management/tabs/contract-tab/contract-tab.component';
import { CountTabComponent } from './contracts-manage/components/contract-management/tabs/count-tab/count-tab.component';
import {
  EnvironmentTabComponent,
} from './contracts-manage/components/contract-management/tabs/environment-tab/environment-tab.component';
import {
  ErrorNotFoundTabComponent,
} from './contracts-manage/components/contract-management/tabs/error-not-found-tab/error-not-found-tab.component';
import {
  EstablishmentTabComponent,
} from './contracts-manage/components/contract-management/tabs/establishment-tab/establishment-tab.component';
import { HistoryTabComponent } from './contracts-manage/components/contract-management/tabs/history-tab/history-tab.component';
import { ContractsManageComponent } from './contracts-manage/contracts-manage.component';
import { ContractsManageModule } from './contracts-manage/contracts-manage.module';

const contractTabsRoutes: Routes = [
  {
    path: ContractsModalTabPath.Contract,
    component: ContractTabComponent,
  },
  {
    path: ContractsModalTabPath.History,
    component: HistoryTabComponent,
  },
  {
    path: ContractsModalTabPath.Close,
    component: CloseTabComponent,
  },
  {
    path: ContractsModalTabPath.Environment,
    component: EnvironmentTabComponent,
  },
  {
    path: ContractsModalTabPath.Counts,
    component: CountTabComponent,
  },
  {
    path: ContractsModalTabPath.Accounting,
    component: AccountingTabComponent,
  },
  {
    path: ContractsModalTabPath.Establishments,
    component: EstablishmentTabComponent,
  },
  {
    path: ContractsModalTabPath.NotFound,
    component: ErrorNotFoundTabComponent,
  },
  {
    path: '**',
    redirectTo: ContractsModalTabPath.Contract,
  },
];

const routes: Routes = [
  {
    path: NavigationPath.Contracts,
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: NavigationPath.ContractsManage,
      },
      {
        path: NavigationPath.ContractsManage,
        component: ContractsManageComponent,
      },
      {
        path: `${ NavigationPath.ContractsManage }/:id`,
        component: ContractManagementComponent,
        children: contractTabsRoutes,
      },
      {
        path: NavigationPath.ContractsDraft,
        component: ContractsDraftComponent,
        children: [
          {
            path: 'form',
            component: ContractsDraftEntryModalComponent,
            canActivate: [OperationsGuard],
            data: { operations: [Operation.CreateContracts] },

          },
          {
            path: ':id/form',
            component: ContractsDraftEntryModalComponent,
            canActivate: [OperationsGuard],
            data: { operations: [Operation.CreateContracts] },
          },
        ],
      },
    ],
  },
];

@NgModule({
  declarations: [ContractsComponent],
  imports: [
    RouterModule.forChild(routes),
    CommonModule,
    ContractsDraftModule,
    ContractsManageModule,
  ],
})
export class ContractsModule { }
