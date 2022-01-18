import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';

import { CountsDashboardCardComponent } from './counts-dashboard-card/counts-dashboard-card.component';
import { CountsDashboardCardListComponent } from './counts-dashboard-card-list.component';

@NgModule({
  declarations: [CountsDashboardCardListComponent, CountsDashboardCardComponent],
  exports: [CountsDashboardCardListComponent],
    imports: [CommonModule, TranslateModule],
})
export class CountsDashboardCardListModule {}
