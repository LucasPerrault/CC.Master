import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiFeederModule, LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPickerModule, LuOptionSelectAllModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { EnvironmentGroupApiSelectComponent } from './environment-group-api-select.component';

@NgModule({
  declarations: [
    EnvironmentGroupApiSelectComponent,
  ],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    LuInputDisplayerModule,
    TranslateModule,
    LuOptionPickerModule,
    LuApiFeederModule,
    LuOptionSelectAllModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuForOptionsModule,
    LuApiSearcherModule,
  ],
  exports: [EnvironmentGroupApiSelectComponent],
})
export class EnvironmentGroupApiSelectModule {
}
