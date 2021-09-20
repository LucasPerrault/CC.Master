import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputDisplayerModule } from '@lucca-front/ng/input';
import {
  LuForOptionsModule,
  LuOptionFeederModule,
  LuOptionItemModule,
  LuOptionPickerModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { BillingMonthSelectComponent } from './billing-month-select.component';

@NgModule({
  declarations: [BillingMonthSelectComponent],
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    LuSelectInputModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputDisplayerModule,
  ],
  exports: [BillingMonthSelectComponent],
})
export class BillingMonthSelectModule { }
