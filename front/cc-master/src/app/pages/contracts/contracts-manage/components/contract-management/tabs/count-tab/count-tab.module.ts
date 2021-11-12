import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { DateRangeSelectModule } from '@cc/common/forms';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CountsDetailDownloadCalloutComponent } from './components/counts-detail-download-callout/counts-detail-download-callout.component';
import { CountsDetailDownloadFormComponent } from './components/counts-detail-download-form/counts-detail-download-form.component';
import { CountsListComponent } from './components/counts-list/counts-list.component';
import { CountsReplayModalComponent } from './components/counts-replay-modal/counts-replay-modal.component';
import { DraftActionsButtonGroupComponent } from './components/draft-actions-button-group/draft-actions-button-group.component';
import { CountTabComponent } from './count-tab.component';
import { CountContractsDataService } from './services/count-contracts-data.service';
import { CountContractsListService } from './services/count-contracts-list.service';
import { CountContractsRestrictionsService } from './services/count-contracts-restrictions.service';

@NgModule({
  declarations: [
    CountTabComponent,
    CountsListComponent,
    CountsDetailDownloadFormComponent,
    CountsDetailDownloadCalloutComponent,
    DraftActionsButtonGroupComponent,
    CountsReplayModalComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    LuTooltipTriggerModule,
    DateRangeSelectModule,
    ReactiveFormsModule,
    FormsModule,
  ],
  providers: [CountContractsListService, CountContractsDataService, CountContractsRestrictionsService],
})
export class CountTabModule { }
