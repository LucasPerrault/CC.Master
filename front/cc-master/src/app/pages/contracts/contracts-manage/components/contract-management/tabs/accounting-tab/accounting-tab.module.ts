import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { AccountingTabComponent } from './accounting-tab.component';
import { AccountOverviewComponent } from './components/account-overview/account-overview.component';
import { AccountingEntryListComponent } from './components/accounting-entry-list/accounting-entry-list.component';
import { AccountingEntryDataService } from './services/accounting-entry-data.service';
import { AccountingEntryListService } from './services/accounting-entry-list.service';

@NgModule({
  declarations: [
    AccountingTabComponent,
    AccountingEntryListComponent,
    AccountOverviewComponent,
  ],
  imports: [
    CommonModule,
    PagingModule,
    TranslateModule,
  ],
  providers: [AccountingEntryListService, AccountingEntryDataService],
})
export class AccountingTabModule { }
