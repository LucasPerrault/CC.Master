import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { TheoreticalMonthRebateComponent } from './theoretical-month-rebate.component';

@NgModule({
  declarations: [TheoreticalMonthRebateComponent],
    imports: [
        CommonModule,
        FormsModule,
        TranslateModule,
        ReactiveFormsModule,
    ],
  exports: [TheoreticalMonthRebateComponent],
})
export class TheoreticalMonthRebateModule { }
