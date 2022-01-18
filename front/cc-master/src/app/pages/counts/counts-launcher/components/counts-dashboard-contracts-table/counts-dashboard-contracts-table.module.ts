import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { LuTooltipTriggerModule } from '@lucca-front/ng/tooltip';

import { CountsDashboardContractsTableComponent } from './counts-dashboard-contracts-table.component';

@NgModule({
  declarations: [CountsDashboardContractsTableComponent],
  exports: [CountsDashboardContractsTableComponent],
    imports: [
        CommonModule,
        LuTooltipTriggerModule,
        TranslateModule,
    ],
})
export class CountsDashboardContractsTableModule {}
