import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { EstablishmentHealthSelectComponent } from './establishment-health-select.component';

@NgModule({
  declarations: [EstablishmentHealthSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    TranslateModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuForOptionsModule,
    LuInputDisplayerModule,
  ],
  exports: [EstablishmentHealthSelectComponent],
})
export class EstablishmentHealthSelectModule { }
