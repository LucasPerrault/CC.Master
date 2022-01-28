import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'cc-counts-dashboard-card',
  templateUrl: './counts-dashboard-card.component.html',
  styleUrls: ['./counts-dashboard-card.component.scss'],
})
export class CountsDashboardCardComponent {
  @Input() public mod: string;
  @Input() public title: string;
  @Input() public totalCount: number;
  @Input() public isLoading: boolean;
  @Input() public isSelected: boolean;

  @Output() public showMoreDetails: EventEmitter<void> = new EventEmitter();

  constructor() { }

}
