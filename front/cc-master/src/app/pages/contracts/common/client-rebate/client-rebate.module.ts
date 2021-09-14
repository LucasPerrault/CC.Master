import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';

import { ClientRebateComponent } from './client-rebate.component';

@NgModule({
  declarations: [ClientRebateComponent],
    imports: [
        CommonModule,
        TranslateModule,
        FormsModule,
        LuDateSelectInputModule,
        ReactiveFormsModule,
    ],
  exports: [ClientRebateComponent],
})
export class ClientRebateModule { }
