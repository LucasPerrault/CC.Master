import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { OfferCurrencyApiSelectComponent } from './offer-currency-api-select.component';
import { OfferCurrencyApiSelectService } from './offer-currency-api-select.service';


@NgModule({
  declarations: [OfferCurrencyApiSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    LuInputDisplayerModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionItemModule,
    LuForOptionsModule,
    ReactiveFormsModule,
    TranslateModule,
    LuApiSearcherModule,
    LuInputClearerModule,
  ],
  exports: [OfferCurrencyApiSelectComponent],
  providers: [OfferCurrencyApiSelectService],
})
export class OfferCurrencyApiSelectModule {
}
