import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';

import { CafeComponent } from './cafe.component';
import { CafeCategoriesService } from './common/services/cafe-categories.service';
import { CriterionFormlyConfigurationService } from './common/services/criterion-formly-configuration.service';
import { FacetAdvancedFilterModule } from './common/services/facets';
import { CafeContactsModule } from './contacts/cafe-contacts.module';
import { CafeEnvironmentsModule } from './environments/cafe-environments.module';
import { EstablishmentsModule } from './establishments/establishments.module';

@NgModule({
  declarations: [CafeComponent],
  imports: [
    CommonModule,
    TranslateModule,
    CafeEnvironmentsModule,
    CafeContactsModule,
    EstablishmentsModule,
    ReactiveFormsModule,
    FacetAdvancedFilterModule,
  ],
  providers: [
    CafeCategoriesService,
    CriterionFormlyConfigurationService,
  ],
})
export class CafeModule { }
