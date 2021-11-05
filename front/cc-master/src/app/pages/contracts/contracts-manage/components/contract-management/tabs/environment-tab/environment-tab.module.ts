import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { EnvironmentApiSelectModule } from '@cc/common/forms';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import {
  EnvironmentLinkedInformationComponent,
} from './components/contract-environment-linked/contract-environment-linked.component';
import {
  EnvironmentCreationCauseSelectComponent,
} from './components/environment-creation-cause-select/environment-creation-cause-select.component';
import { EnvironmentLinkSelectComponent } from './components/environment-link-select/environment-link-select.component';
import { EnvironmentTabComponent } from './environment-tab.component';
import { ContractEnvironmentService } from './services/contract-environment.service';
import { ContractEnvironmentActionRestrictionsService } from './services/contract-environment-action-restrictions.service';
import { EnvironmentCreationCauseService } from './services/environment-creation-cause.service';

@NgModule({
  declarations: [
    EnvironmentTabComponent,
    EnvironmentLinkedInformationComponent,
    EnvironmentLinkSelectComponent,
    EnvironmentCreationCauseSelectComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    LuTooltipTriggerModule,
    ReactiveFormsModule,
    EnvironmentApiSelectModule,
  ],
  providers: [
    ContractEnvironmentService,
    ContractEnvironmentActionRestrictionsService,
    EnvironmentCreationCauseService,
  ],
})
export class EnvironmentTabModule { }
