import { CommonModule, CurrencyPipe } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CountsPageTemplateModule } from '../common/counts-page-template/counts-page-template.module';
import { CountAdditionalColumnSelectModule } from './components/count-additional-column-select/count-additional-column-select.module';
import { CountsFilterModule } from './components/counts-filter/counts-filter.module';
import { CountsListComponent } from './components/counts-list/counts-list.component';
import { CountsTablePageComponent } from './counts-table-page.component';
import { CountsApiMappingService } from './services/counts-api-mapping.service';
import { CountsDataService } from './services/counts-data.service';
import { CountsFilterRoutingService } from './services/counts-filter-routing.service';
import { CountsRoutingService } from './services/counts-routing.service';

@NgModule({
  declarations: [
    CountsTablePageComponent,
    CountsListComponent,
  ],
    imports: [
        CommonModule,
        CountsPageTemplateModule,
        TranslateModule,
        PagingModule,
        LuTooltipTriggerModule,
        CountsFilterModule,
        ReactiveFormsModule,
        CountAdditionalColumnSelectModule,
    ],
  providers: [
    CountsDataService,
    CountsFilterRoutingService,
    CountsRoutingService,
    CountsApiMappingService,
    CurrencyPipe,
  ],
})
export class CountsTablePageModule { }
