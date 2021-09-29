import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { CafeEnvironmentsComponent } from './cafe-environments.component';
import { CountryListModalModule } from './components/country-list-modal/country-list-modal.module';
import {
  EnvironmentAdditionalColumnSelectModule,
} from './components/environment-additional-column-select/environment-additional-column-select.module';
import { EnvironmentListComponent } from './components/environment-list/environment-list.component';
import { EnvironmentDataService } from './services/environment-data.service';
import { CafeEnvironmentConfiguration } from './cafe-environments.configuration';
import { EnvironmentAdvancedFilterConfiguration } from './advanced-filter/environment-advanced-filter.configuration';
import { EnvironmentContactFormlyConfiguration } from './advanced-filter/environment-contact-formly.configuration';
import { EnvironmentAdvancedFilterApiMappingService } from './advanced-filter/environment-advanced-filter-api-mapping.service';

@NgModule({
  declarations: [
    CafeEnvironmentsComponent,
    EnvironmentListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
    CountryListModalModule,
    EnvironmentAdditionalColumnSelectModule,
    ReactiveFormsModule,
  ],
  exports: [
    CafeEnvironmentsComponent,
  ],
  providers: [
    EnvironmentDataService,
    CafeEnvironmentConfiguration,
    EnvironmentAdvancedFilterConfiguration,
    EnvironmentContactFormlyConfiguration,
    EnvironmentAdvancedFilterApiMappingService,
  ],
})
export class CafeEnvironmentsModule { }
