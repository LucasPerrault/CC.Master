import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { DistributorApiSelectModule } from '@cc/common/forms/select/distributor-api-select/v4';
import { LuApiSelectInputModule } from '@lucca-front/ng/api';

import { DemoUserApiSelectModule } from '../selects';
import { DemoFiltersComponent } from './demo-filters.component';

@NgModule({
  declarations: [DemoFiltersComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DemoUserApiSelectModule,
    TranslateModule,
    LuApiSelectInputModule,
    DistributorApiSelectModule,
  ],
  exports: [DemoFiltersComponent],
})
export class DemoFiltersModule {
}
