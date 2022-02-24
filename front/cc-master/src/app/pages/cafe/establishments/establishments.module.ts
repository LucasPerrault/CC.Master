import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { CafePageFilterTemplateModule } from '../common/components/cafe-page-filter-template/cafe-page-filter-template.module';
import { CafePageTemplateModule } from '../common/components/cafe-page-template/cafe-page-template.module';
import { FacetsAndColumnsApiSelectModule } from '../common/forms/select/facets-and-columns-api-select/facets-and-columns-api-select.module';
import {
  EstablishmentAdvancedFilterApiMappingService,
  EstablishmentAdvancedFilterConfiguration,
  EstablishmentFormlyConfiguration,
} from './advanced-filter';
import { EstablishmentListComponent } from './components/establishment-list/establishment-list.component';
import { EstablishmentsComponent } from './establishments.component';
import { EstablishmentsDataService } from './services/establishments-data.service';
import { FacetPipeModule } from '../common/pipes/facet.pipe';

@NgModule({
  declarations: [EstablishmentsComponent, EstablishmentListComponent],
    imports: [
        CommonModule,
        TranslateModule,
        PagingModule,
        CafePageTemplateModule,
        CafePageFilterTemplateModule,
        ReactiveFormsModule,
        FacetsAndColumnsApiSelectModule,
        FacetPipeModule,
    ],
  exports: [EstablishmentsComponent],
  providers: [
    EstablishmentsDataService,
    EstablishmentAdvancedFilterConfiguration,
    EstablishmentAdvancedFilterApiMappingService,
    EstablishmentFormlyConfiguration,
  ],
})
export class EstablishmentsModule { }
