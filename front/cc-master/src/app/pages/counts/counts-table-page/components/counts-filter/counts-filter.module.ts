import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import {
  ClientApiSelectModule,
  DateRangeSelectModule,
  DistributorApiSelectModule, EnvironmentGroupApiSelectModule,
  OfferApiSelectModule,
  ProductApiSelectModule,
} from '@cc/common/forms';

import { CountsFilterComponent } from './counts-filter.component';

@NgModule({
  declarations: [CountsFilterComponent],
  exports: [CountsFilterComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DateRangeSelectModule,
    ClientApiSelectModule,
    DistributorApiSelectModule,
    OfferApiSelectModule,
    TranslateModule,
    ProductApiSelectModule,
    EnvironmentGroupApiSelectModule,
  ],
})
export class CountsFilterModule {}
