import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import { OfferImportComponent } from './offer-import.component';

@NgModule({
  declarations: [
    OfferImportComponent,
  ],
  imports: [
    CommonModule,
    OfferPageTemplateModule,
    TranslateModule,
  ],
})
export class OfferImportModule { }
