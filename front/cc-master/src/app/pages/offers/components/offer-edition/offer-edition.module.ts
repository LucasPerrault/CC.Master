import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';

import { EditablePriceGridModule } from '../editable-price-grid/editable-price-grid.module';
import { OfferPageTemplateModule } from '../offer-page-template/offer-page-template.module';
import { OfferEditionComponent } from './offer-edition.component';
import { OfferEditionFormModule } from './offer-edition-tab/offer-edition-form/offer-edition-form.module';
import { OfferEditionRestrictionsService } from './offer-edition-restrictions.service';
import { OfferEditionValidationContextService } from './offer-edition-validation-context.service';
import { OfferEditionTabComponent } from './offer-edition-tab/offer-edition-tab.component';


@NgModule({
  declarations: [OfferEditionComponent, OfferEditionTabComponent],
  imports: [
    CommonModule,
    OfferPageTemplateModule,
    TranslateModule,
    ReactiveFormsModule,
    EditablePriceGridModule,
    OfferEditionFormModule,
  ],
  providers: [OfferEditionRestrictionsService, OfferEditionValidationContextService],
})
export class OfferEditionModule {
}
