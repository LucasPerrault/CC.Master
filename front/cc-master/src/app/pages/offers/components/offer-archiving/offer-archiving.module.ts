import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { OfferArchivingComponent } from './offer-archiving.component';

@NgModule({
  declarations: [
    OfferArchivingComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
  ],
})
export class OfferArchivingModule { }
