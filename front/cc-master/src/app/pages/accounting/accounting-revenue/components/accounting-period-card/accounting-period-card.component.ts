import { Component, Input } from '@angular/core';

@Component({
  selector: 'cc-accounting-period-card',
  templateUrl: './accounting-period-card.component.html',
})
export class AccountingPeriodCardComponent {
  @Input() public accountingPeriod: Date;
  @Input() public isLoading: boolean;

  constructor() { }
}
