import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';

import { ICountListEntry } from '../../models/count-list-entry.interface';

@Component({
  selector: 'cc-draft-actions-button-group',
  templateUrl: './draft-actions-button-group.component.html',
  styleUrls: ['./draft-actions-button-group.component.scss'],
})
export class DraftActionsButtonGroupComponent {
  @Input() showDraftCounts: boolean;
  @Input() chargeButtonState: string;
  @Input() deleteButtonState: string;
  @Input() draftCounts: ICountListEntry[];
  @Output() chargeDraft: EventEmitter<void> = new EventEmitter<void>();
  @Output() deleteDraft: EventEmitter<void> = new EventEmitter<void>();
  @Output() toggleDraftDisplay: EventEmitter<void> = new EventEmitter<void>();

  public get hasDraftCounts(): boolean {
    return !!this.draftCounts?.length;
  }

  constructor(private rightsService: RightsService) { }

  public canChargeDraftCount(): boolean {
    return this.rightsService.hasOperation(Operation.CreateCounts);
  }

  public toggleDraftCountsDisplay(): void {
    this.toggleDraftDisplay.emit();
  }

  public chargeDraftCounts(): void {
    this.chargeDraft.emit();
  }

  public deleteDraftCount(): void {
    this.deleteDraft.emit();
  }
}
