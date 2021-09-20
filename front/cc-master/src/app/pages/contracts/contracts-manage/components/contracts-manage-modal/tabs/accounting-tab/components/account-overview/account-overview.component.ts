import { Component, EventEmitter, Input, Output } from '@angular/core';

import { IAccount } from '../../enums/account-type.enum';

@Component({
  selector: 'cc-account-overview',
  templateUrl: './account-overview.component.html',
  styleUrls: ['./account-overview.component.scss'],
})
export class AccountOverviewComponent {
  @Input() account: IAccount;
  @Input() balance: number;
  @Output() updateFilters: EventEmitter<boolean> = new EventEmitter<boolean>();

  public isExpanded = false;
  public showLetteredEntriesDisplay = false;

  constructor() { }

  public toggleAccountDisplay(): void {
    this.isExpanded = !this.isExpanded;
  }

  public toggleLetteredAccountingDisplay(): void {
    this.showLetteredEntriesDisplay = !this.showLetteredEntriesDisplay;
    this.updateFilters.emit(this.showLetteredEntriesDisplay);
  }
}
