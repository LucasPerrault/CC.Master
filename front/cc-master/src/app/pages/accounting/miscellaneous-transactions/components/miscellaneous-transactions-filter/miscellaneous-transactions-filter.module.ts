import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { BillingEntitySelectModule } from '@cc/common/forms';

import { ContractApiSelectModule } from '../contract-api-select/contract-api-select.module';
import { MiscellaneousTransactionsFilterComponent } from './miscellaneous-transactions-filter.component';

@NgModule({
  declarations: [
    MiscellaneousTransactionsFilterComponent,
  ],
  exports: [
    MiscellaneousTransactionsFilterComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ContractApiSelectModule,
    TranslateModule,
    BillingEntitySelectModule,
  ],
})
export class MiscellaneousTransactionsFilterModule { }
