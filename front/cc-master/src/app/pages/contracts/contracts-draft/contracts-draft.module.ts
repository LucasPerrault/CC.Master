import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@cc/aspects/translate';
import { ClientApiSelectModule, DistributorApiSelectModule, OfferApiSelectModule, ProductApiSelectModule } from '@cc/common/forms';
import { QueriesModule } from '@cc/common/queries';
import { ContractsService, MinimalBillingService } from '@cc/domain/billing/contracts';
import { DistributorsService } from '@cc/domain/billing/distributors';
import { ALuDateAdapter, LuNativeDateAdapter } from '@lucca-front/ng/core';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuModalModule } from '@lucca-front/ng/modal';
import { LuSidepanelModule } from '@lucca-front/ng/sidepanel';
import { LuTooltipModule } from '@lucca-front/ng/tooltip';

import {
  BillingFrequencySelectModule,
  ClientRebateModule,
  CommentModule,
  DistributorFilterButtonGroupModule,
  MinimalBillingPercentageModule,
  PriceGridModalModule,
  ProductChipModule,
  TheoreticalDraftCountModule,
  TheoreticalMonthRebateModule,
} from '../common';
import { ContractsDraftDeletionModalComponent } from './components/contracts-draft-deletion-modal/contracts-draft-deletion-modal.component';
import { ContractsDraftFilterComponent } from './components/contracts-draft-filter/contracts-draft-filter.component';
import { ContractsDraftFormComponent } from './components/contracts-draft-form/contracts-draft-form.component';
import { ContractsDraftListComponent } from './components/contracts-draft-list/contracts-draft-list.component';
import {
  ContractsDraftEntryModalComponent,
  ContractsDraftModalComponent,
} from './components/contracts-draft-modal/contracts-draft-modal.component';
import { ContractsDraftComponent } from './contracts-draft.component';
import { ContractsDraftService } from './services';
import { ContractsDraftApiMappingService } from './services/contracts-draft-api-mapping.service';
import { ContractsDraftListService } from './services/contracts-draft-list.service';

@NgModule({
  declarations: [
    ContractsDraftComponent,
    ContractsDraftListComponent,
    ContractsDraftFilterComponent,
    ContractsDraftEntryModalComponent,
    ContractsDraftModalComponent,
    ContractsDraftFormComponent,
    ContractsDraftDeletionModalComponent,
  ],
  imports: [
    CommonModule,
    ProductChipModule,
    TranslateModule,
    DistributorFilterButtonGroupModule,
    FormsModule,
    LuSidepanelModule,
    LuTooltipModule,
    ReactiveFormsModule,
    BillingFrequencySelectModule,
    DistributorApiSelectModule,
    ClientApiSelectModule,
    ProductApiSelectModule,
    OfferApiSelectModule,
    PriceGridModalModule,
    LuModalModule,
    TheoreticalDraftCountModule,
    TheoreticalMonthRebateModule,
    LuDateSelectInputModule,
    ClientRebateModule,
    MinimalBillingPercentageModule,
    QueriesModule,
    RouterModule,
    CommentModule,
  ],
  providers: [
    ContractsDraftService,
    ContractsDraftListService,
    ContractsDraftApiMappingService,
    ContractsService,
    MinimalBillingService,
    DistributorsService,
    LuNativeDateAdapter,
    { provide: ALuDateAdapter, useClass: LuNativeDateAdapter },
  ],
})
export class ContractsDraftModule { }
