import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { OfferCurrencySelectComponent } from './offer-currency-select.component';


@NgModule({
  declarations: [OfferCurrencySelectComponent],
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
  exports: [OfferCurrencySelectComponent],
})
export class OfferCurrencySelectModule {
}
