import { Component, EventEmitter, Input, Output } from '@angular/core';
import { endOfMonth, isBefore, startOfMonth } from 'date-fns';

@Component({
  selector: 'cc-accounting-period-card',
  templateUrl: './accounting-period-card.component.html',
})
export class AccountingPeriodCardComponent {
  @Input() public accountingPeriod: Date;
  @Input() public isLoading: boolean;
  @Input() public closeButtonState: string;
  @Output() public closeCurrentPeriod: EventEmitter<Date> = new EventEmitter();

  public get canCloseCurrentPeriod(): boolean {
    const today = new Date();
    return isBefore(endOfMonth(this.accountingPeriod), startOfMonth(today));
  }

  constructor() { }

  public close(): void {
    this.closeCurrentPeriod.emit(this.accountingPeriod);
  }
}
