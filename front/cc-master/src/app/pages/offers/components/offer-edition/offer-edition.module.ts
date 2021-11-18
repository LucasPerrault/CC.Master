import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { PriceListModule } from '@cc/domain/billing/offers';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuModalModule } from '@lucca-front/ng/modal';

import { OfferValidationContextDataService } from '../../services/offer-validation-context-data.service';
import { EditablePriceGridModule } from '../editable-price-grid/editable-price-grid.module';
import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import { OfferEditionComponent } from './offer-edition.component';
import { OfferEditionFormModule } from './offer-edition-tab/offer-edition-form/offer-edition-form.module';
import { OfferEditionTabComponent } from './offer-edition-tab/offer-edition-tab.component';
import { OfferNotFoundTabComponent } from './offer-not-found-tab/offer-not-found-tab.component';
import {
  OfferPriceListCreationModalComponent,
} from './offer-price-lists-tab/offer-price-list-creation-modal/offer-price-list-creation-modal.component';
import {
  OfferPriceListDeletionModalComponent,
} from './offer-price-lists-tab/offer-price-list-deletion-modal/offer-price-list-deletion-modal.component';
import {
  OfferPriceListEditionModalComponent,
} from './offer-price-lists-tab/offer-price-list-edition-modal/offer-price-list-edition-modal.component';
import { OfferPriceListsTabComponent } from './offer-price-lists-tab/offer-price-lists-tab.component';
import { OfferPriceListsTabService } from './offer-price-lists-tab/offer-price-lists-tab.service';
import { OffersEditionStoreService } from './offers-edition-store.service';

@NgModule({
  declarations: [
    OfferEditionComponent,
    OfferEditionTabComponent,
    OfferPriceListsTabComponent,
    OfferPriceListCreationModalComponent,
    OfferPriceListEditionModalComponent,
    OfferPriceListDeletionModalComponent,
    OfferNotFoundTabComponent,
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
  providers: [
    OfferValidationContextDataService,
    OfferPriceListsTabService,
    OffersEditionStoreService,
  ],
})
export class OfferEditionModule {
}
