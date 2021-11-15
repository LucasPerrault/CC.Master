import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { OfferPageTemplateComponent } from './offer-page-template.component';
import { RouterModule } from '@angular/router';

@NgModule({
  declarations: [OfferPageTemplateComponent],
  imports: [
    CommonModule,
    TranslateModule,
    RouterModule,
  ],
  exports: [OfferPageTemplateComponent],
})
export class OfferPageTemplateModule {
}
