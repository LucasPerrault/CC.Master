import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { CloseContractReasonSelectComponent } from './close-contract-reason-select.component';

@NgModule({
  declarations: [CloseContractReasonSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    LuOptionPickerModule,
    FormsModule,
    LuOptionItemModule,
    LuInputDisplayerModule,
    LuOptionFeederModule,
    LuInputClearerModule,
    LuForOptionsModule,
    TranslateModule,
  ],
  exports: [CloseContractReasonSelectComponent],
})
export class CloseContractReasonSelectModule { }
