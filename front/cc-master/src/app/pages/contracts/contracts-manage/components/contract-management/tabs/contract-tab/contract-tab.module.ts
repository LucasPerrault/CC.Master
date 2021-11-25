import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { ClientApiSelectModule, DistributorApiSelectModule, OfferApiSelectModule, ProductApiSelectModule } from '@cc/common/forms';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuModalModule } from '@lucca-front/ng/modal';

import {
  BillingFrequencySelectModule,
  ClientRebateModule, CommentModule, MinimalBillingPercentageModule,
  TheoreticalDraftCountModule,
  TheoreticalMonthRebateModule,
} from '../../../../../common';
import { ClientInfoModalModule } from './components/contract-tab-form/client-info-modal/client-info-modal.module';
import { ContractTabFormComponent } from './components/contract-tab-form/contract-tab-form.component';
import { ContractTabComponent } from './contract-tab.component';
import { ContractActionRestrictionsService } from './services/contract-action-restrictions.service.';
import { ContractTabService } from './services/contract-tab.service';

@NgModule({
  declarations: [ContractTabComponent, ContractTabFormComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    BillingFrequencySelectModule,
    LuDateSelectInputModule,
    TranslateModule,
    DistributorApiSelectModule,
    ClientApiSelectModule,
    ProductApiSelectModule,
    OfferApiSelectModule,
    TheoreticalDraftCountModule,
    TheoreticalMonthRebateModule,
    ClientRebateModule,
    MinimalBillingPercentageModule,
    CommentModule,
    LuModalModule,
    ClientInfoModalModule,
  ],
  providers: [
    ContractTabService,
    ContractActionRestrictionsService,
  ],
})
export class ContractTabModule { }
