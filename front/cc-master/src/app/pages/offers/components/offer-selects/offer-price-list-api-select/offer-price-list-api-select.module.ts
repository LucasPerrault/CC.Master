import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionItemModule, LuOptionPagerModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { OfferPriceListApiSelectComponent } from './offer-price-list-api-select.component';

@NgModule({
  declarations: [OfferPriceListApiSelectComponent],
  imports: [
    CommonModule,
    LuSelectInputModule,
    FormsModule,
    LuInputDisplayerModule,
    TranslateModule,
    LuOptionPickerModule,
    LuApiSearcherModule,
    LuOptionPagerModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputClearerModule,
    ReactiveFormsModule,
  ],
  exports: [OfferPriceListApiSelectComponent],
})
export class OfferPriceListApiSelectModule { }
