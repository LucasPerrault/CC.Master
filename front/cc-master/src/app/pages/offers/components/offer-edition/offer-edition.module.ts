import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { PriceListModule } from '@cc/domain/billing/offers';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuModalModule } from '@lucca-front/ng/modal';

import { EditablePriceGridModule } from '../editable-price-grid/editable-price-grid.module';
import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import { OfferEditionComponent } from './offer-edition.component';
import { OfferEditionRestrictionsService } from './offer-edition-restrictions.service';
import { OfferEditionFormModule } from './offer-edition-tab/offer-edition-form/offer-edition-form.module';
import { OfferEditionTabComponent } from './offer-edition-tab/offer-edition-tab.component';
import { OfferEditionValidationContextService } from './offer-edition-validation-context.service';
import {
  OfferPriceListCreationModalComponent,
} from './offer-price-lists-tab/offer-price-list-creation-modal/offer-price-list-creation-modal.component';
import { OfferPriceListsTabComponent } from './offer-price-lists-tab/offer-price-lists-tab.component';

@NgModule({
  declarations: [
    OfferEditionComponent,
    OfferEditionTabComponent,
    OfferPriceListsTabComponent,
    OfferPriceListCreationModalComponent,
  ],
  imports: [
    CommonModule,
    OfferPageTemplateModule,
    TranslateModule,
    ReactiveFormsModule,
    EditablePriceGridModule,
    OfferEditionFormModule,
    RouterModule,
    LuModalModule,
    PriceListModule,
    LuDateSelectInputModule,
  ],
  providers: [OfferEditionRestrictionsService, OfferEditionValidationContextService],
})
export class OfferEditionModule {
}
