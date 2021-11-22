import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { OfferDeletionModule } from './components/offer-deletion/offer-deletion.module';
import { OfferFiltersModule } from './components/offer-filters/offer-filters.module';
import { OfferListComponent } from './components/offer-list/offer-list.component';
import { OffersComponent } from './offers.component';
import { OffersRoutingModule } from './offers-routing.module';
import { OfferListService } from './services/offer-list.service';
import { OfferRestrictionsService } from './services/offer-restrictions.service';
import { OfferUsageStoreService } from './services/offer-usage-store.service';
import { OffersApiMappingService } from './services/offers-api-mapping.service';
import { OffersDataService } from './services/offers-data.service';
import { PriceListsDataService } from './services/price-lists-data.service';

@NgModule({
  declarations: [
    OffersComponent,
    OfferListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
    LuTooltipTriggerModule,
    ReactiveFormsModule,
    OfferFiltersModule,
    OfferDeletionModule,
    OffersRoutingModule,
  ],
  providers: [
    OffersDataService,
    PriceListsDataService,
    OffersApiMappingService,
    OfferListService,
    OfferRestrictionsService,
    OfferUsageStoreService,
  ],
})
export class OffersModule { }
