import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuDateSelectInputModule } from '@lucca-front/ng/date';

import { CountsPageTemplateModule } from '../common/counts-page-template/counts-page-template.module';
import { CountsDashboardCardListModule } from './components/counts-dashboard-card-list/counts-dashboard-card-list.module';
import { CountsDashboardContractsTableModule } from './components/counts-dashboard-contracts-table/counts-dashboard-contracts-table.module';
import { CountsLauncherComponent } from './counts-launcher.component';
import { CountsDataService } from './services/counts-data.service';
import { CountsLauncherService } from './services/counts-launcher.service';

@NgModule({
  declarations: [CountsLauncherComponent],
    imports: [
        CommonModule,
        CountsPageTemplateModule,
        LuDateSelectInputModule,
        ReactiveFormsModule,
        CountsDashboardCardListModule,
        CountsDashboardContractsTableModule,
        PagingModule,
        TranslateModule,
    ],
  providers: [CountsDataService, CountsLauncherService],
})
export class CountsLauncherModule {
}
