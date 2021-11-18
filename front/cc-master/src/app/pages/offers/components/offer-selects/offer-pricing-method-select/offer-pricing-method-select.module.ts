import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import {
  LuForOptionsModule,
  LuOptionFeederModule,
  LuOptionItemModule,
  LuOptionPagerModule,
  LuOptionPickerModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { OfferPricingMethodSelectComponent } from './offer-pricing-method-select.component';

@NgModule({
  declarations: [
    OfferPricingMethodSelectComponent,
  ],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    TranslateModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuForOptionsModule,
    LuOptionItemModule,
    LuApiSearcherModule,
    LuOptionPagerModule,
    LuInputClearerModule,
  ],
  exports: [OfferPricingMethodSelectComponent],
})
export class OfferPricingMethodSelectModule { }
