import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { OfferApiSelectModule, ProductApiSelectModule } from '@cc/common/forms';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { EditablePriceGridModule } from '../editable-price-grid/editable-price-grid.module';
import {
  OfferBillingModeSelectModule,
  OfferBillingUnitSelectModule,
  OfferCurrencyApiSelectModule,
  OfferForecastMethodApiSelectModule,
  OfferPricingMethodApiSelectModule,
  OfferTagApiSelectModule,
} from '../offer-selects';
import { OfferTagAutocompleteSelectModule } from '../offer-selects/offer-tag-autocomplete-select/offer-tag-autocomplete-select.module';
import { OfferFormComponent } from './offer-form.component';

@NgModule({
    declarations: [
        OfferFormComponent,
    ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslateModule,
    ProductApiSelectModule,
    OfferBillingModeSelectModule,
    OfferCurrencyApiSelectModule,
    OfferTagApiSelectModule,
    OfferBillingUnitSelectModule,
    OfferPricingMethodApiSelectModule,
    OfferForecastMethodApiSelectModule,
    OfferTagAutocompleteSelectModule,
    OfferApiSelectModule,
    EditablePriceGridModule,
    LuTooltipTriggerModule,
  ],
    exports: [OfferFormComponent],
})
export class OfferFormModule { }
