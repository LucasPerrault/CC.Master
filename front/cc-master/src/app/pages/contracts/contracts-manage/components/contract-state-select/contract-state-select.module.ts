import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { ContractStateSelectComponent } from './contract-state-select.component';

@NgModule({
  declarations: [ContractStateSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    TranslateModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuInputDisplayerModule,
    LuForOptionsModule,
  ],
  exports: [ContractStateSelectComponent],
})
export class ContractStateSelectModule { }
