import { Component, Input } from '@angular/core';

import { ISyncRevenueInfo } from '../../models/sync-revenue-info.interface';

@Component({
  selector: 'cc-accounting-sync-revenue-card',
  templateUrl: './accounting-sync-revenue-card.component.html',
})
export class AccountingSyncRevenueCardComponent {
  @Input() public syncRevenueInfo: ISyncRevenueInfo;
  @Input() public isLoading: boolean;

  constructor() { }
}
