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

import { OfferForecastMethodApiSelectComponent } from './offer-forecast-method-api-select.component';
import { OfferForecastMethodApiSelectService } from './offer-forecast-method-api-select.service';

@NgModule({
  declarations: [
    OfferForecastMethodApiSelectComponent,
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
  exports: [OfferForecastMethodApiSelectComponent],
  providers: [OfferForecastMethodApiSelectService],
})
export class OfferForecastMethodApiSelectModule { }
