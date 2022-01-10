import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import {
  ContractWithoutEnvironmentCalloutComponent,
} from './components/contract-without-environment-callout/contract-without-environment-callout.component';
import { EstablishmentTypeFilterModule } from './components/establishment-filters/establiment-type-filter/establishment-type-filter.module';
import { EstablishmentFiltersModule } from './components/establishment-filters/establishment-filters.module';
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
import { EstablishmentContractDataService } from './services/establishment-contract-data.service';
import { EstablishmentListActionsService } from './services/establishment-list-actions.service';
import { EstablishmentListEntriesService } from './services/establishment-list-entries.service';
import { EstablishmentProductStoreService } from './services/establishment-product-store.service';
import { EstablishmentProductStoreDataService } from './services/establishment-product-store-data.service';
import { EstablishmentsDataService } from './services/establishments-data.service';
import { EstablishmentsTimelineService } from './services/establishments-timeline.service';

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
    EstablishmentFiltersModule,
  ],
  providers: [
    EstablishmentsDataService,
    AttachmentsActionRestrictionsService,
    EstablishmentContractDataService,
    EstablishmentListActionsService,
    EstablishmentProductStoreService,
    EstablishmentProductStoreDataService,
    AttachmentsTimelineService,
    EstablishmentsTimelineService,
    EstablishmentListEntriesService,
  ],
})
export class EstablishmentTabModule { }
