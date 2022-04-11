import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { CafePageFilterTemplateModule } from '../common/components/cafe-page-filter-template/cafe-page-filter-template.module';
import { CafePageTemplateModule } from '../common/components/cafe-page-template/cafe-page-template.module';
import { FacetsAndColumnsApiSelectModule } from '../common/forms/select/facets-and-columns-api-select';
import { FacetPipeModule } from '../common/pipes/facet.pipe';
import { ColumnAutoSelectionModule } from '../common/services/column-auto-selection';
import {
  EnvironmentAdvancedFilterApiMappingService,
  EnvironmentAdvancedFilterConfiguration,
  EnvironmentFormlyConfiguration,
} from './advanced-filter';
import { CafeEnvironmentsComponent } from './cafe-environments.component';
import { EnvironmentListComponent } from './components/environment-list/environment-list.component';
import { EnvironmentDataService } from './services/environment-data.service';

@NgModule({
  declarations: [
    CafeEnvironmentsComponent,
    EnvironmentListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
    ReactiveFormsModule,
    CafePageTemplateModule,
    CafePageFilterTemplateModule,
    FacetsAndColumnsApiSelectModule,
    FacetPipeModule,
    ColumnAutoSelectionModule,
  ],
  exports: [
    CafeEnvironmentsComponent,
  ],
  providers: [
    EnvironmentDataService,
    EnvironmentAdvancedFilterConfiguration,
    EnvironmentFormlyConfiguration,
    EnvironmentAdvancedFilterApiMappingService,
  ],
})
export class CafeEnvironmentsModule {}
