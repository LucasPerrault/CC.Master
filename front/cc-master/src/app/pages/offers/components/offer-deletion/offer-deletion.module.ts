import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { OfferDeletionComponent } from './offer-deletion.component';

@NgModule({
  declarations: [
    OfferDeletionComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
  ],
})
export class OfferDeletionModule { }
