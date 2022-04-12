import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSelectInputModule } from '@lucca-front/ng/api';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuNumberModule } from '@lucca-front/ng/number';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { ContractApiSelectModule } from './components/contract-api-select/contract-api-select.module';
import {
  MiscTransactionCreationModalComponent,
} from './components/misc-transaction-creation-modal/misc-transaction-creation-modal.component';
import { MiscTransactionsListComponent } from './components/misc-transactions-list/misc-transactions-list.component';
import {
  MiscellaneousTransactionsFilterModule,
} from './components/miscellaneous-transactions-filter/miscellaneous-transactions-filter.module';
import { MiscellaneousTransactionsComponent } from './miscellaneous-transactions.component';
import { AccountingAmountPipe } from './pipes/accounting-amount.pipe';
import { MiscTransactionsApiMappingService } from './services/misc-transactions-api-mapping.service';
import { MiscellaneousTransactionsService } from './services/miscellaneous-transactions.service';



@NgModule({
  declarations: [
    MiscellaneousTransactionsComponent,
    MiscTransactionsListComponent,
    AccountingAmountPipe,
    MiscTransactionCreationModalComponent,
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
        LuDateSelectInputModule,
        MiscellaneousTransactionsFilterModule,
    ],
  providers: [
    MiscellaneousTransactionsService,
    MiscTransactionsApiMappingService,
  ],
})
export class MiscellaneousTransactionsModule { }
