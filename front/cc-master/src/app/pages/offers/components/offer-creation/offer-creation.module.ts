import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { EditablePriceGridModule } from '../editable-price-grid/editable-price-grid.module';
import { OfferFormModule } from '../offer-form/offer-form.module';
import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import { OfferCreationComponent } from './offer-creation.component';


@NgModule({
  declarations: [OfferCreationComponent],
  imports: [
    CommonModule,
    OfferPageTemplateModule,
    TranslateModule,
    OfferFormModule,
    ReactiveFormsModule,
    EditablePriceGridModule,
  ],
})
export class OfferCreationModule {
}
