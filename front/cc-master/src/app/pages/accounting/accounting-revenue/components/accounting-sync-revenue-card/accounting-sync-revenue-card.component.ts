import { Component, EventEmitter, Input, Output } from '@angular/core';

import { ISyncRevenueInfo } from '../../models/sync-revenue-info.interface';
import { CurrentSyncRevenueInfo } from '../../services/sync-revenue.service';

@Component({
  selector: 'cc-accounting-sync-revenue-card',
  templateUrl: './accounting-sync-revenue-card.component.html',
})
export class AccountingSyncRevenueCardComponent {
  @Input() public syncRevenueInfo: CurrentSyncRevenueInfo;
  @Input() public isLoading: boolean;
  @Input() public syncButtonState: string;
  @Output() public syncRevenue: EventEmitter<void> = new EventEmitter();

  public get isDisabled(): boolean {
    return this.isLoading || this.syncRevenueInfo.syncRevenue.lineCount === 0;
  }

  constructor() { }

  public synchronise(): void {
    this.syncRevenue.emit();
  }
}
