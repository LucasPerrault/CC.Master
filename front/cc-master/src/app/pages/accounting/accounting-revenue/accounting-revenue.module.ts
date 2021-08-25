import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { AccountingRevenueComponent } from './accounting-revenue.component';
import { AccountingPeriodService } from './services/accounting-period.service';
import { SyncRevenueService } from './services/sync-revenue.service';
import { AccountingSyncRevenueCardComponent } from './components/accounting-sync-revenue-card/accounting-sync-revenue-card.component';
import { AccountingPeriodCardComponent } from './components/accounting-period-card/accounting-period-card.component';

@NgModule({
  declarations: [
    AccountingRevenueComponent,
    AccountingSyncRevenueCardComponent,
    AccountingPeriodCardComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
  ],
  providers: [
    AccountingPeriodService,
    SyncRevenueService,
  ],
})
export class AccountingRevenueModule { }
