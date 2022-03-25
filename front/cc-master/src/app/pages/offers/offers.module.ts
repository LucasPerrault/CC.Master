import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuDropdownItemModule, LuDropdownPanelModule, LuDropdownTriggerModule } from '@lucca-front/ng/dropdown';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { OfferArchivingModule } from './components/offer-archiving/offer-archiving.module';
import { OfferFiltersModule } from './components/offer-filters/offer-filters.module';
import { OfferListComponent } from './components/offer-list/offer-list.component';
import { OffersComponent } from './offers.component';
import { OffersPageComponent } from './offers-page.component';
import { OffersRoutingModule } from './offers-routing.module';
import { OfferListService } from './services/offer-list.service';
import { OfferRestrictionsService } from './services/offer-restrictions.service';
import { OfferUsageStoreService } from './services/offer-usage-store.service';
import { OffersApiMappingService } from './services/offers-api-mapping.service';
import { OffersDataService } from './services/offers-data.service';
import { OffersFilterRoutingService } from './services/offers-filter-routing.service';
import { OffersRoutingService } from './services/offers-routing.service';
import { PriceListsDataService } from './services/price-lists-data.service';

@NgModule({
  declarations: [
    OffersComponent,
    OfferListComponent,
    OffersPageComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
    LuTooltipTriggerModule,
    ReactiveFormsModule,
    OfferFiltersModule,
    OfferArchivingModule,
    OffersRoutingModule,
    LuDropdownPanelModule,
    LuDropdownTriggerModule,
    LuDropdownItemModule,
  ],
  providers: [
    OffersDataService,
    PriceListsDataService,
    OffersApiMappingService,
    OfferListService,
    OfferRestrictionsService,
    OfferUsageStoreService,
    OffersRoutingService,
    OffersFilterRoutingService,
  ],
})
export class OffersModule { }
