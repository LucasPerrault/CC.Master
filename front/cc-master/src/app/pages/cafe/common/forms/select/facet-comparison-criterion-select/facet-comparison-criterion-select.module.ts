import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import {
  LuForOptionsModule,
  LuOptionFeederModule,
  LuOptionItemModule,
  LuOptionPickerModule,
  LuOptionSearcherModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { FormlyModule } from '@ngx-formly/core';

import { FacetComparisonCriterionSelectComponent } from './facet-comparison-criterion-select.component';
import { FacetApiSelectModule } from '../facet-api-select/facet-api-select.module';



@NgModule({
  declarations: [FacetComparisonCriterionSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    TranslateModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionSearcherModule,
    LuOptionItemModule,
    LuInputClearerModule,
    LuForOptionsModule,
    LuInputDisplayerModule,
    FormlyModule,
    FacetApiSelectModule,
  ],
  exports: [FacetComparisonCriterionSelectComponent],
})
export class FacetComparisonCriterionSelectModule { }
