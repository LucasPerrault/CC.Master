import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import { OfferImportComponent } from './offer-import.component';
import { OfferUploadComponent } from './offer-upload/offer-upload.component';
import { OfferImportTableComponent } from './offer-import-table/offer-import-table.component';

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
  ],
})
export class OfferImportModule { }
