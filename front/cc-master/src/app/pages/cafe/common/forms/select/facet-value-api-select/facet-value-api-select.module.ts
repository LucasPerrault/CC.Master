import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiModule } from '@lucca-front/ng/api';
import { LuInputModule } from '@lucca-front/ng/input';
import {
  LuOptionModule,
  LuOptionPagerModule,
  LuOptionPickerModule,
  LuOptionSearcherModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { FormlyModule } from '@ngx-formly/core';

import { FacetValueApiSelectComponent } from './facet-value-api-select.component';

@NgModule({
  declarations: [FacetValueApiSelectComponent],
  imports: [
    FormsModule,
    LuSelectInputModule,
    LuOptionPickerModule,
    LuOptionSearcherModule,
    LuOptionPagerModule,
    LuOptionModule,
    LuInputModule,
    LuApiModule,
    TranslateModule,
    CommonModule,
    ReactiveFormsModule,
    FormlyModule,
  ],
  exports: [FacetValueApiSelectComponent],
})
export class FacetValueApiSelectModule { }
