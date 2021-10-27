import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiFeederModule, LuApiSearcherModule } from '@lucca-front/ng/api';
import { LuInputClearerModule, LuInputDisplayerModule } from '@lucca-front/ng/input';
import {
  LuForOptionsModule,
  LuOptionFeederModule,
  LuOptionItemModule,
  LuOptionPagerModule,
  LuOptionPickerModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';

import { OfferTagAutocompleteSelectComponent } from './offer-tag-autocomplete-select.component';

@NgModule({
  declarations: [
    OfferTagAutocompleteSelectComponent,
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
    LuApiFeederModule,
  ],
  exports: [OfferTagAutocompleteSelectComponent],
})
export class OfferTagAutocompleteSelectModule { }
