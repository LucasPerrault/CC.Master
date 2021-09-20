import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Operation, OperationsGuard } from '@cc/aspects/rights';
import { NavigationPath } from '@cc/common/navigation';

import { ContractsComponent } from './contracts.component';
import { ContractsDraftEntryModalComponent } from './contracts-draft/components/contracts-draft-modal/contracts-draft-modal.component';
import { ContractsDraftComponent } from './contracts-draft/contracts-draft.component';
import { ContractsDraftModule } from './contracts-draft/contracts-draft.module';
import { ContractsModalTabPath } from './contracts-manage/components/contracts-manage-modal/constants/contracts-modal-tab-path.enum';
import { ContractsManageEntryModalComponent } from './contracts-manage/components/contracts-manage-modal/contracts-manage-modal.component';
import { AccountingTabComponent } from './contracts-manage/components/contracts-manage-modal/tabs/accounting-tab/accounting-tab.component';
import { CloseTabComponent } from './contracts-manage/components/contracts-manage-modal/tabs/close-tab/close-tab.component';
import { ContractTabComponent } from './contracts-manage/components/contracts-manage-modal/tabs/contract-tab/contract-tab.component';
import { CountTabComponent } from './contracts-manage/components/contracts-manage-modal/tabs/count-tab/count-tab.component';
import {
  EnvironmentTabComponent,
} from './contracts-manage/components/contracts-manage-modal/tabs/environment-tab/environment-tab.component';
import {
  ErrorNotFoundTabComponent,
} from './contracts-manage/components/contracts-manage-modal/tabs/error-not-found-tab/error-not-found-tab.component';
import {
  EstablishmentTabComponent,
} from './contracts-manage/components/contracts-manage-modal/tabs/establishment-tab/establishment-tab.component';
import { HistoryTabComponent } from './contracts-manage/components/contracts-manage-modal/tabs/history-tab/history-tab.component';
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
        children: [
          {
            path: ':id',
            component: ContractsManageEntryModalComponent,
            children: contractTabsRoutes,
          },
        ],
      },
      {
        path: 'draft',
        component: ContractsDraftComponent,
        children: [
          {
            path: 'create',
            component: ContractsDraftEntryModalComponent,
            canActivate: [OperationsGuard],
            data: { operations: [Operation.CreateContracts] },

          },
          {
            path: ':id/create',
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
