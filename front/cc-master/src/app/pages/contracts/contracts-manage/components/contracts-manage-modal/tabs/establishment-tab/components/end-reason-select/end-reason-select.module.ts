import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { EndReasonSelectComponent } from './end-reason-select.component';

@NgModule({
  declarations: [EndReasonSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuApiSearcherModule,
    LuOptionItemModule,
    LuForOptionsModule,
    TranslateModule,
  ],
  exports: [EndReasonSelectComponent],
})
export class EndReasonSelectModule { }
