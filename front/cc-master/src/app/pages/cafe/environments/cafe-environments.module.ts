import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { EnvironmentAdvancedFilterConfiguration } from './advanced-filter/environment-advanced-filter.configuration';
import { EnvironmentAdvancedFilterApiMappingService } from './advanced-filter/environment-advanced-filter-api-mapping.service';
import { EnvironmentFormlyConfiguration } from './advanced-filter/environment-formly-configuration.service';
import { CafeEnvironmentsComponent } from './cafe-environments.component';
import { CafeEnvironmentConfiguration } from './cafe-environments.configuration';
import {
  EnvironmentAdditionalColumnSelectModule,
} from './components/environment-additional-column-select/environment-additional-column-select.module';
import { EnvironmentListComponent } from './components/environment-list/environment-list.component';
import { EnvironmentDataService } from './services/environment-data.service';
import { CafePageTemplateModule } from '../common/components/cafe-page-template/cafe-page-template.module';
import { CafeFiltersModule } from '../common/cafe-filters/cafe-filters.module';
import { CafePageFilterTemplateModule } from '../common/components/cafe-page-filter-template/cafe-page-filter-template.module';

@NgModule({
  declarations: [
    CafeEnvironmentsComponent,
    EnvironmentListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
    EnvironmentAdditionalColumnSelectModule,
    ReactiveFormsModule,
    CafePageTemplateModule,
    CafeFiltersModule,
    CafePageFilterTemplateModule,
  ],
  exports: [
    CafeEnvironmentsComponent,
  ],
  providers: [
    EnvironmentDataService,
    CafeEnvironmentConfiguration,
    EnvironmentAdvancedFilterConfiguration,
    EnvironmentFormlyConfiguration,
    EnvironmentAdvancedFilterApiMappingService,
  ],
})
export class CafeEnvironmentsModule { }
