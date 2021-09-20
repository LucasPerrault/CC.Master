import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { CountsService } from '@cc/domain/billing/counts';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';

import { CloseTabComponent } from './close-tab.component';
import { CloseContractReasonSelectModule } from './components/close-contract-reason-select/close-contract-reason-select.module';
import { ClosureCancellationComponent } from './components/closure-cancellation/closure-cancellation.component';
import { ContractClosureFormComponent } from './components/contract-closure-form/contract-closure-form.component';
import {
  ContractClosureFormInformationComponent,
} from './components/contract-closure-form-information/contract-closure-form-information.component';
import { CloseContractService } from './services/close-contract.service';
import { CloseContractFormService } from './services/close-contract-form.service';

@NgModule({
  declarations: [CloseTabComponent, ClosureCancellationComponent, ContractClosureFormComponent, ContractClosureFormInformationComponent],
  imports: [
    CommonModule,
    CloseContractReasonSelectModule,
    LuDateSelectInputModule,
    TranslateModule,
    ReactiveFormsModule,
  ],
  providers: [CloseContractService, CloseContractFormService, CountsService],
})
export class CloseTabModule { }
