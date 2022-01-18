import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';
import { LuModalModule } from '@lucca-front/ng/modal';

import { CountsPageTemplateModule } from '../common/counts-page-template/counts-page-template.module';
import { CountsDashboardCardListModule } from './components/counts-dashboard-card-list/counts-dashboard-card-list.module';
import { CountsDashboardContractsTableModule } from './components/counts-dashboard-contracts-table/counts-dashboard-contracts-table.module';
import { CountsProcessLauncherModalModule } from './components/counts-process-launcher-modal/counts-process-launcher-modal.module';
import { CountsLauncherComponent } from './counts-launcher.component';
import { CountsDataService } from './services/counts-data.service';
import { CountsLauncherService } from './services/counts-launcher.service';
import { CountsProcessDataService } from './services/counts-process-data.service';

@NgModule({
  declarations: [CountsLauncherComponent],
  imports: [
    CommonModule,
    CountsPageTemplateModule,
    LuDateSelectInputModule,
    ReactiveFormsModule,
    CountsDashboardCardListModule,
    CountsDashboardContractsTableModule,
    CountsProcessLauncherModalModule,
    PagingModule,
    TranslateModule,
    LuModalModule,
  ],
  providers: [CountsDataService, CountsLauncherService, CountsProcessDataService],
})
export class CountsLauncherModule {
}
