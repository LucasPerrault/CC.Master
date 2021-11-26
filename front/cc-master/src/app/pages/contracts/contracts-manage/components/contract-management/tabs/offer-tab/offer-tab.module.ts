import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { OfferTabComponent } from './offer-tab.component';
import { OfferTabDataService } from './services/offer-tab-data.service';
import { LuApiSelectInputModule } from '@lucca-front/ng/api';
import { ReactiveFormsModule } from '@angular/forms';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';
import { TranslateModule } from '@cc/aspects/translate';
import { SimilarOfferApiSelectModule } from './components/similar-offer-api-select/similar-offer-api-select.module';

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
