import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { MinimalBillingPercentageComponent } from './minimal-billing-percentage.component';

@NgModule({
  declarations: [MinimalBillingPercentageComponent],
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    LuTooltipTriggerModule,
    ReactiveFormsModule,
  ],
  exports: [MinimalBillingPercentageComponent],
})
export class MinimalBillingPercentageModule { }
