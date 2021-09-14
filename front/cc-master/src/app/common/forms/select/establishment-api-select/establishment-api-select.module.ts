import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPagerModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { EstablishmentApiSelectComponent } from './establishment-api-select.component';

@NgModule({
  declarations: [EstablishmentApiSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    LuInputDisplayerModule,
    TranslateModule,
    LuOptionPickerModule,
    LuApiSearcherModule,
    LuOptionPagerModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputClearerModule,
  ],
  exports: [EstablishmentApiSelectComponent],
})
export class EstablishmentApiSelectModule { }
