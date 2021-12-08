import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import { LuForOptionsModule, LuOptionFeederModule, LuOptionItemModule, LuOptionPickerModule } from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { OfferBillingUnitSelectComponent } from './offer-billing-unit-select.component';

@NgModule({
  declarations: [
    OfferBillingUnitSelectComponent,
  ],
  exports: [
    OfferBillingUnitSelectComponent,
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
export class OfferBillingUnitSelectModule {
}
