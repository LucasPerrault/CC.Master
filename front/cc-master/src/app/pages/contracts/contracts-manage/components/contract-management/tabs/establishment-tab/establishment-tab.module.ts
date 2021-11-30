import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import {
  ContractWithoutEnvironmentCalloutComponent,
} from './components/contract-without-environment-callout/contract-without-environment-callout.component';
import { EstablishmentTypeFilterModule } from './components/establiment-type-filter/establishment-type-filter.module';
import { EstablishmentListComponent } from './components/establishment-list/establishment-list.component';
import {
  EstablishmentListActionsMultipleComponent,
} from './components/establishment-list-actions-multiple/establishment-list-actions-multiple.component';
import {
  EstablishmentListActionsSingleComponent,
} from './components/establishment-list-actions-single/establishment-list-actions-single.component';
import { EstablishmentTypeSectionComponent } from './components/establishment-type-section/establishment-type-section.component';
import { EstablishmentActionModalsModule } from './components/modals/establishment-action-modals.module';
import { EstablishmentTabComponent } from './establishment-tab.component';
import { AttachmentsActionRestrictionsService } from './services/attachments-action-restrictions.service';
import { AttachmentsTimelineService } from './services/attachments-timeline.service';
import { EstablishmentActionContextService } from './services/establishment-action-context.service';
import { EstablishmentContractDataService } from './services/establishment-contract-data.service';
import { EstablishmentListActionsService } from './services/establishment-list-actions.service';
import { EstablishmentTypeService } from './services/establishment-type.service';
import { EstablishmentsDataService } from './services/establishments-data.service';
import { EstablishmentsWithAttachmentsService } from './services/establishments-with-attachments.service';

@NgModule({
  declarations: [
    EstablishmentTabComponent,
    EstablishmentTypeSectionComponent,
    EstablishmentListComponent,
    EstablishmentListActionsSingleComponent,
    ContractWithoutEnvironmentCalloutComponent,
    EstablishmentListActionsMultipleComponent,
  ],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        TranslateModule,
        LuTooltipTriggerModule,
        EstablishmentActionModalsModule,
        EstablishmentTypeFilterModule,
    ],
  providers: [
    EstablishmentsDataService,
    EstablishmentsWithAttachmentsService,
    EstablishmentTypeService,
    AttachmentsTimelineService,
    AttachmentsActionRestrictionsService,
    EstablishmentContractDataService,
    EstablishmentListActionsService,
    EstablishmentActionContextService,
  ],
})
export class EstablishmentTabModule { }
