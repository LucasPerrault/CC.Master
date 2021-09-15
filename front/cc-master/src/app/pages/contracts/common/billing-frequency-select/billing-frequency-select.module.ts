import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { BillingMonthSelectModule } from '../billing-month-select/billing-month-select.module';
import { BillingFrequencySelectComponent } from './billing-frequency-select.component';

@NgModule({
  declarations: [BillingFrequencySelectComponent],
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    BillingMonthSelectModule,
  ],
  exports: [BillingFrequencySelectComponent],
})
export class BillingFrequencySelectModule { }
