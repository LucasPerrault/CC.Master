import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { RightsModule } from '@cc/aspects/rights';
import { TranslateModule } from '@cc/aspects/translate';

import { ContractsManageModalComponent } from './contracts-manage-modal.component';
import { ContractsManageModalService } from './contracts-manage-modal.service';
import { ContractsManageModalDataService } from './contracts-manage-modal-data.service';
import { AccountingTabModule } from './tabs/accounting-tab/accounting-tab.module';
import { CloseTabModule } from './tabs/close-tab/close-tab.module';
import { ContractTabModule } from './tabs/contract-tab/contract-tab.module';
import { CountTabModule } from './tabs/count-tab/count-tab.module';
import { EnvironmentTabModule } from './tabs/environment-tab/environment-tab.module';
import { ErrorNotFoundTabModule } from './tabs/error-not-found-tab/error-not-found-tab.module';
import { EstablishmentTabModule } from './tabs/establishment-tab/establishment-tab.module';
import { HistoryTabModule } from './tabs/history-tab/history-tab.module';

@NgModule({
  declarations: [ContractsManageModalComponent],
  imports: [
    CommonModule,
    RightsModule,
    TranslateModule,
    RouterModule,
    ContractTabModule,
    HistoryTabModule,
    CloseTabModule,
    EnvironmentTabModule,
    CountTabModule,
    AccountingTabModule,
    EstablishmentTabModule,
    ErrorNotFoundTabModule,
  ],
  providers: [ContractsManageModalService, ContractsManageModalDataService],
})
export class ContractsManageModalModule { }
