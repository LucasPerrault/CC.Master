import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { ProductApiSelectModule } from '@cc/common/forms';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuModalModule } from '@lucca-front/ng/modal';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { EditablePriceGridModule } from '../editable-price-grid/editable-price-grid.module';
import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import {
  OfferBillingModeSelectModule,
  OfferBillingUnitSelectModule,
  OfferCurrencySelectModule,
  OfferForecastMethodApiSelectModule,
  OfferPricingMethodApiSelectModule,
  OfferTagApiSelectModule,
} from '../offer-selects';
import { ImportedPriceListsModalComponent } from './imported-price-lists-modal/imported-price-lists-modal.component';
import { OfferImportComponent } from './offer-import.component';
import { OfferImportTableComponent } from './offer-import-table/offer-import-table.component';
import { OfferUploadComponent } from './offer-upload/offer-upload.component';
import { OfferUploadService } from './offer-upload/offer-upload.service';

@NgModule({
  declarations: [
    OfferImportComponent,
    OfferUploadComponent,
    OfferImportTableComponent,
    ImportedPriceListsModalComponent,
  ],
  imports: [
    CommonModule,
    OfferPageTemplateModule,
    TranslateModule,
    ProductApiSelectModule,
    ReactiveFormsModule,
    OfferBillingUnitSelectModule,
    OfferTagApiSelectModule,
    OfferBillingModeSelectModule,
    OfferPricingMethodApiSelectModule,
    OfferForecastMethodApiSelectModule,
    OfferCurrencySelectModule,
    LuModalModule,
    LuDateSelectInputModule,
    EditablePriceGridModule,
    LuTooltipTriggerModule,
  ],
  providers: [
    OfferUploadService,
  ],
})
export class OfferImportModule { }
