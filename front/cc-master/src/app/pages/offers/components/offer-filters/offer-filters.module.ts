import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { ProductApiSelectModule } from '@cc/common/forms';

import {
  OfferBillingModeSelectModule,
  OfferCurrencySelectModule,
  OfferTagApiSelectModule,
} from '../offer-selects';
import { OfferFiltersComponent } from './offer-filters.component';
import { OfferStateFilterModule } from './offer-state-filter';

@NgModule({
  declarations: [OfferFiltersComponent],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        OfferTagApiSelectModule,
        ProductApiSelectModule,
        OfferCurrencySelectModule,
        OfferBillingModeSelectModule,
        TranslateModule,
        OfferStateFilterModule,
    ],
  exports: [OfferFiltersComponent],
})
export class OfferFiltersModule { }
