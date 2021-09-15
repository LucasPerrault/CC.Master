import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule, LuApiSelectInputModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPagerModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { DistributorApiSelectComponent } from './distributor-api-select.component';

@NgModule({
  declarations: [DistributorApiSelectComponent],
  imports: [
    CommonModule,
    LuApiSelectInputModule,
    FormsModule,
    LuSelectInputModule,
    TranslateModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuApiSearcherModule,
    LuOptionPagerModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputClearerModule,
  ],
  exports: [DistributorApiSelectComponent],
})
export class DistributorApiSelectModule { }
