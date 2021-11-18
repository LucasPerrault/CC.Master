import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { OfferApiSelectModule, ProductApiSelectModule } from '@cc/common/forms';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { EditablePriceGridModule } from '../../../editable-price-grid/editable-price-grid.module';
import {
  OfferBillingModeSelectModule,
  OfferBillingUnitSelectModule,
  OfferCurrencySelectModule,
  OfferForecastMethodSelectModule,
  OfferPricingMethodSelectModule, OfferTagApiSelectModule,
  OfferTagAutocompleteSelectModule,
} from '../../../offer-selects';
import { OfferEditionFormComponent } from './offer-edition-form.component';

@NgModule({
  declarations: [
    OfferEditionFormComponent,
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslateModule,
    ProductApiSelectModule,
    OfferBillingModeSelectModule,
    OfferCurrencySelectModule,
    OfferTagApiSelectModule,
    OfferBillingUnitSelectModule,
    OfferPricingMethodSelectModule,
    OfferForecastMethodSelectModule,
    OfferTagAutocompleteSelectModule,
    OfferApiSelectModule,
    EditablePriceGridModule,
    LuTooltipTriggerModule,
    LuDateSelectInputModule,
  ],
  exports: [OfferEditionFormComponent],
})
export class OfferEditionFormModule {
}
