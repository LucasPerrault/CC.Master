import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { LuNumberModule } from '@lucca-front/ng/number';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { MiscTransactionsListComponent } from './components/misc-transactions-list/misc-transactions-list.component';
import { MiscellaneousTransactionsComponent } from './miscellaneous-transactions.component';
import { AccountingAmountPipe } from './pipes/accounting-amount.pipe';
import { MiscellaneousTransactionsService } from './services/miscellaneous-transactions.service';



@NgModule({
  declarations: [
    MiscellaneousTransactionsComponent,
    MiscTransactionsListComponent,
    AccountingAmountPipe,
  ],
  imports: [
    CommonModule,
    LuNumberModule,
    LuTooltipTriggerModule,
    TranslateModule,
  ],
  providers: [MiscellaneousTransactionsService],
})
export class MiscellaneousTransactionsModule { }
