import { Component, Input } from '@angular/core';

import { ICountsDashboard } from '../../models/counts-dashboard.interface';

@Component({
  selector: 'cc-counts-dashboard-card-list',
  templateUrl: './counts-dashboard-card-list.component.html',
})
export class CountsDashboardCardListComponent {
  @Input() public dashboard: ICountsDashboard;
}
