import { CommonModule, DatePipe } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { DownloadModule } from '@cc/common/download';
import { PagingModule } from '@cc/common/paging';
import { SortService } from '@cc/common/sort';
import { ClientsService } from '@cc/domain/billing/clients';
import { ContractsService } from '@cc/domain/billing/contracts';
import { EstablishmentsService } from '@cc/domain/billing/establishments';
import { OffersService, ProductsService } from '@cc/domain/billing/offers';
import { EnvironmentsService } from '@cc/domain/environments';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { ProductChipModule } from '../common';
import {
  ContractAdditionalColumnSelectModule,
} from './components/contract-additional-column-select/contract-additional-column-select.module';
import { ContractManagementModule } from './components/contract-management/contract-management.module';
import { ContractsManageFilterModule } from './components/contracts-manage-filter/contracts-manage-filter.module';
import { ContractsManageListComponent } from './components/contracts-manage-list/contracts-manage-list.component';
import { ContractsManageComponent } from './contracts-manage.component';
import { ContractsApiMappingService } from './services/contracts-api-mapping.service';
import { ContractsFilterRoutingService } from './services/contracts-filter-routing.service';
import { ContractsListService } from './services/contracts-list.service';
import { ContractsRoutingService } from './services/contracts-routing.service';

@NgModule({
  declarations: [ContractsManageComponent, ContractsManageListComponent],
  imports: [
      CommonModule,
      RouterModule,
      PagingModule,
      ProductChipModule,
      TranslateModule,
      ReactiveFormsModule,
      DownloadModule,
      ContractsManageFilterModule,
      ContractManagementModule,
      ContractAdditionalColumnSelectModule,
      LuTooltipTriggerModule,
  ],
  providers: [
    ContractsListService,
    DatePipe,
    ContractsService,
    ContractsApiMappingService,
    SortService,
    ContractsRoutingService,
    ContractsFilterRoutingService,
    ClientsService,
    ProductsService,
    OffersService,
    EnvironmentsService,
    EstablishmentsService,
  ],
})
export class ContractsManageModule { }
