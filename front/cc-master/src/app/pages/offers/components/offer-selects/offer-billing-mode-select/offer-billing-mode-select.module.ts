import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { OfferBillingModeSelectComponent } from './offer-billing-mode-select.component';

@NgModule({
  declarations: [
    OfferBillingModeSelectComponent,
  ],
  exports: [
    OfferBillingModeSelectComponent,
  ],
  imports: [
    CommonModule,
    LuSelectInputModule,
    ReactiveFormsModule,
    LuOptionPickerModule,
    LuOptionFeederModule,
    LuOptionItemModule,
    LuForOptionsModule,
    LuInputDisplayerModule,
    TranslateModule,
    LuInputClearerModule,
  ],
})
export class OfferBillingModeSelectModule {
}
