import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPagerModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { ClientApiSelectComponent } from './client-api-select.component';

@NgModule({
  declarations: [ClientApiSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    TranslateModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuApiSearcherModule,
    LuOptionPagerModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuForOptionsModule,
  ],
  exports: [ClientApiSelectComponent],
})
export class ClientApiSelectModule { }
