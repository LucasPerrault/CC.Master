import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { DateRangeSelectModule } from '@cc/common/forms';
import { LuDropdownItemModule, LuDropdownPanelModule, LuDropdownTriggerModule } from '@lucca-front/ng/dropdown';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CountsDetailDownloadModalComponent } from './components/counts-detail-download-modal/counts-detail-download-modal.component';
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
    CountsDetailDownloadModalComponent,
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
    LuDropdownItemModule,
    LuDropdownPanelModule,
    LuDropdownTriggerModule,
  ],
  providers: [CountContractsListService, CountContractsDataService, CountContractsRestrictionsService],
})
export class CountTabModule { }
