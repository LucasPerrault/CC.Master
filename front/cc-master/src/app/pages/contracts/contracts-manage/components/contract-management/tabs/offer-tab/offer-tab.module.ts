import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuApiSelectInputModule } from '@lucca-front/ng/api';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { SimilarOfferApiSelectModule } from './components/similar-offer-api-select/similar-offer-api-select.module';
import { OfferTabComponent } from './offer-tab.component';
import { OfferTabDataService } from './services/offer-tab-data.service';

@NgModule({
  declarations: [
    OfferTabComponent,
  ],
  imports: [
    CommonModule,
    LuApiSelectInputModule,
    ReactiveFormsModule,
    LuTooltipTriggerModule,
    TranslateModule,
    SimilarOfferApiSelectModule,
  ],
  providers: [OfferTabDataService],
})
export class OfferTabModule { }
