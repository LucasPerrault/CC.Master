import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';
import { TranslatePipe } from '@cc/aspects/translate';

import { AttachmentEndReason, getAttachmentEndReason } from '../../constants/attachment-end-reason.const';
import { IContractEstablishment } from '../../models/contract-establishment.interface';
import { IEstablishmentActionsContext } from '../../models/establishment-actions-context.interface';
import { IListEntry, LifecycleStep, ListEntryType } from '../../models/establishment-list-entry.interface';
import { AttachmentsActionRestrictionsService } from '../../services/attachments-action-restrictions.service';
import { EstablishmentsTimelineService } from '../../services/establishments-timeline.service';

@Component({
  selector: 'cc-establishment-list',
  templateUrl: './establishment-list.component.html',
  styleUrls: ['./establishment-list.component.scss'],
})
export class EstablishmentListComponent {
  @Input() public type: ListEntryType;
  @Input() public entries: IListEntry[];
  @Input() public context: IEstablishmentActionsContext;

  public get areAllSelected(): boolean {
    return this.selectedEntries.length === this.entries.length && this.entries.length !== 0;
  }

  public selectedEntries: IListEntry[] = [];

  public get isLinked(): boolean {
    return [ListEntryType.LinkedToAnotherContract, ListEntryType.LinkedToThisContract].includes(this.type);
  }

  public get isEmpty(): boolean {
    return !this.entries.length;
  }

  public listEntryType = ListEntryType;

  public get canReadValidationContext(): boolean {
    return this.restrictionsService.canReadValidationContext;
  }

  constructor(
    private translatePipe: TranslatePipe,
    private datePipe: DatePipe,
    private rightsService: RightsService,
    private restrictionsService: AttachmentsActionRestrictionsService,
    private timelineService: EstablishmentsTimelineService,
  ) { }

  public isType(type: ListEntryType): boolean {
    return this.type === type;
  }

  public trackBy(index: number, entry: IListEntry): number {
    return entry.establishment?.id;
  }

  public canEditAttachments(): boolean {
    return this.rightsService.hasOperation(Operation.EditContractsEntities);
  }

  public selectAll(): void {
    this.selectedEntries = this.areAllSelected ? [] : this.entries;
  }

  public isSelected(entry: IListEntry): boolean {
    return !!this.selectedEntries.find(e => e.establishment?.id === entry.establishment?.id);
  }

  public select(entry: IListEntry): void {
    this.selectedEntries = this.isSelected(entry)
      ? this.selectedEntries.filter(e => e.establishment?.id !== entry.establishment?.id)
      : [...this.selectedEntries, entry];
  }

  public getEndReason(reason: AttachmentEndReason): string {
    const translationKey = getAttachmentEndReason(reason)?.name;
    return this.translatePipe.transform(translationKey);
  }

  public getLifecycleStepDescription(lifecycleStep: LifecycleStep, establishment: IContractEstablishment): string {
    if (this.timelineService.isErrorLifecycleStep(lifecycleStep)) {
      return this.getErrorLifecycleDescription(lifecycleStep, establishment);
    }

    if (lifecycleStep === LifecycleStep.InProgress) {
      return this.translatePipe.transform('front_contractPage_establishments_state_inProgress');
    }

    if (lifecycleStep === LifecycleStep.StartInTheFuture) {
      return this.translatePipe.transform('front_contractPage_establishments_state_futureActivation');
    }
  }

  private getErrorLifecycleDescription(lifecycleStep: LifecycleStep, establishment: IContractEstablishment): string {
    if (lifecycleStep === LifecycleStep.Inactive) {
      return this.translatePipe.transform('front_contractPage_establishments_errorState_deleted');
    }

    if (lifecycleStep === LifecycleStep.WaitingFirstActivation) {
      return this.translatePipe.transform('front_contractPage_establishments_errorState_waitingFirstActivation');
    }

    const lastCoveringEnd = this.timelineService.getLastAttachmentBeforeToday(establishment)?.end;
    const nextCoveringStart = this.timelineService.getNextAttachmentAfterToday(establishment)?.start;

    if (lifecycleStep === LifecycleStep.NotLinkedSince) {
      const since = this.datePipe.transform(new Date(lastCoveringEnd), 'dd/MM/yyyy');
      return this.translatePipe.transform('front_contractPage_establishments_errorState_notLinkedSince', { since });
    }

    const from = this.datePipe.transform(new Date(lastCoveringEnd), 'dd/MM/yyyy');
    const to = this.datePipe.transform(nextCoveringStart, 'dd/MM/yyyy');
    return this.translatePipe.transform('front_contractPage_establishments_errorState_notCoveredBetween', { from, to });
  }
}
