import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSelectInputModule } from '@lucca-front/ng/api';
import { LuNumberModule } from '@lucca-front/ng/number';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { MiscTransactionsListComponent } from './components/misc-transactions-list/misc-transactions-list.component';
import { MiscellaneousTransactionsComponent } from './miscellaneous-transactions.component';
import { AccountingAmountPipe } from './pipes/accounting-amount.pipe';
import { MiscellaneousTransactionsService } from './services/miscellaneous-transactions.service';
import { ContractApiSelectModule } from './components/contract-api-select/contract-api-select.module';



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
    LuApiSelectInputModule,
    FormsModule,
    ReactiveFormsModule,
    ContractApiSelectModule,
  ],
  providers: [MiscellaneousTransactionsService],
})
export class MiscellaneousTransactionsModule { }
