import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { ProductApiSelectModule } from '@cc/common/forms';

import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import {
  OfferBillingModeSelectModule,
  OfferBillingUnitSelectModule,
  OfferCurrencySelectModule,
  OfferForecastMethodApiSelectModule,
  OfferPricingMethodApiSelectModule,
  OfferTagApiSelectModule,
} from '../offer-selects';
import { OfferImportComponent } from './offer-import.component';
import { OfferImportTableComponent } from './offer-import-table/offer-import-table.component';
import { OfferUploadComponent } from './offer-upload/offer-upload.component';

@NgModule({
  declarations: [
    OfferImportComponent,
    OfferUploadComponent,
    OfferImportTableComponent,
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
  ],
})
export class OfferImportModule { }
