import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'cc-advanced-filters-actions',
  templateUrl: './advanced-filters-actions.component.html',
})
export class AdvancedFiltersActionsComponent {
  @Input() public disabled = false;
  @Output() public add: EventEmitter<void> = new EventEmitter<void>();
  @Output() public clear: EventEmitter<void> = new EventEmitter<void>();

  constructor() { }

  public addFilter(): void {
    this.add.emit();
  }

  public clearFilters(): void {
    this.clear.emit();
  }
}
