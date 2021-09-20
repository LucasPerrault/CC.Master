import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import {
  ClientApiSelectModule, DateRangeSelectModule,
  DistributorApiSelectModule,
  EnvironmentApiSelectModule, EstablishmentApiSelectModule,
  OfferApiSelectModule,
  ProductApiSelectModule,
} from '@cc/common/forms';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';

import { DistributorFilterButtonGroupModule } from '../../../common';
import { ContractStateSelectModule } from '../contract-state-select/contract-state-select.module';
import { EstablishmentHealthSelectModule } from '../establishment-health-select/establishment-health-select.module';
import { ContractsManageFilterComponent } from './contracts-manage-filter.component';

@NgModule({
  declarations: [ContractsManageFilterComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DistributorFilterButtonGroupModule,
    ClientApiSelectModule,
    TranslateModule,
    DistributorApiSelectModule,
    ProductApiSelectModule,
    OfferApiSelectModule,
    EnvironmentApiSelectModule,
    ContractStateSelectModule,
    LuDateSelectInputModule,
    DateRangeSelectModule,
    EstablishmentApiSelectModule,
    EstablishmentHealthSelectModule,
  ],
  exports: [ContractsManageFilterComponent],
})
export class ContractsManageFilterModule { }
