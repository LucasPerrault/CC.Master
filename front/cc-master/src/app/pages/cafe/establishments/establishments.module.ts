import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { CafePageFilterTemplateModule } from '../common/components/cafe-page-filter-template/cafe-page-filter-template.module';
import { CafePageTemplateModule } from '../common/components/cafe-page-template/cafe-page-template.module';
import {
  EstablishmentAdvancedFilterApiMappingService,
  EstablishmentAdvancedFilterConfiguration,
  EstablishmentFormlyConfiguration,
} from './advanced-filter';
import { EstablishmentListComponent } from './components/establishment-list/establishment-list.component';
import { EstablishmentsComponent } from './establishments.component';
import { EstablishmentsDataService } from './services/establishments-data.service';

@NgModule({
  declarations: [EstablishmentsComponent, EstablishmentListComponent],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
    CafePageTemplateModule,
    CafePageFilterTemplateModule,
    ReactiveFormsModule,
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
