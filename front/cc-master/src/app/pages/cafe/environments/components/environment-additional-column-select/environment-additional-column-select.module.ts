import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { EnvironmentAdditionalColumnSelectComponent } from './environment-additional-column-select.component';


@NgModule({
  declarations: [EnvironmentAdditionalColumnSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputClearerModule,
    TranslateModule,
  ],
  exports: [EnvironmentAdditionalColumnSelectComponent],
})
export class EnvironmentAdditionalColumnSelectModule { }
