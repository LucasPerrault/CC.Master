import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';
import { TranslatePipe } from '@cc/aspects/translate';

import { getAttachmentEndReason } from '../../constants/attachment-end-reason.const';
import { EstablishmentType } from '../../constants/establishment-type.enum';
import { IContractCount } from '../../models/contract-count.interface';
import { IContractEstablishment } from '../../models/contract-establishment.interface';
import { IEstablishmentActionsContext } from '../../models/establishment-actions-context.interface';
import { IEstablishmentAttachment } from '../../models/establishment-attachment.interface';
import { IEstablishmentContract } from '../../models/establishment-contract.interface';
import { IEstablishmentWithAttachments } from '../../models/establishment-with-attachments.interface';
import { AttachmentsTimelineService } from '../../services/attachments-timeline.service';

@Component({
  selector: 'cc-establishment-list',
  templateUrl: './establishment-list.component.html',
  styleUrls: ['./establishment-list.component.scss'],
})
export class EstablishmentListComponent {
  @Input() public type: EstablishmentType;
  @Input() public entries: IEstablishmentWithAttachments[];
  @Input() public contract: IEstablishmentContract;
  @Input() public realCounts: IContractCount[];

  public get selectedEstablishments(): IContractEstablishment[] {
    return this.selectedEntries.map(e => e.establishment);
  }

  public get selectedAttachments(): IEstablishmentAttachment[] {
    return this.selectedEntries.map(e => e.currentAttachment || e.nextAttachment);
  }

  public get areAllSelected(): boolean {
    return this.selectedEntries.length === this.entries.length;
  }

  public get actionsContext(): IEstablishmentActionsContext {
    return {
      contract: this.contract,
      realCounts: this.realCounts,
      lastCountPeriod: this.lastCountPeriod,
      establishmentType: this.type,
    };
  }

  public selectedEntries: IEstablishmentWithAttachments[] = [];

  public get lastCountPeriod(): Date {
    const countsAscSorted = this.realCounts.sort((a, b) =>
      new Date(a.countPeriod).getTime() - new Date(b.countPeriod).getTime());

    const lastCount = countsAscSorted.pop();
    return !!lastCount ? new Date(lastCount.countPeriod) : null;
  }

  public get isLinked(): boolean {
    return [EstablishmentType.LinkedToAnotherContract, EstablishmentType.LinkedToContract].includes(this.type);
  }

  public get isEmpty(): boolean {
    return !this.entries.length;
  }

  public establishmentType = EstablishmentType;

  constructor(
    private translatePipe: TranslatePipe,
    private datePipe: DatePipe,
    private rightsService: RightsService,
    private timelineService: AttachmentsTimelineService,
  ) { }

  public isType(type: EstablishmentType): boolean {
    return this.type === type;
  }

  public trackBy(index: number, entry: IEstablishmentWithAttachments): number {
    return entry.establishment.id;
  }

  public canEditAttachments(): boolean {
    return this.rightsService.hasOperation(Operation.EditContractsEntities);
  }

  public getAttachment(entry: IEstablishmentWithAttachments): IEstablishmentAttachment {
    return entry.currentAttachment || entry.nextAttachment;
  }

  public getAttachmentEndReason(entry: IEstablishmentWithAttachments): string {
    const attachment = this.getAttachment(entry);
    const translationKey = getAttachmentEndReason(attachment.endReason)?.name;
    return this.translatePipe.transform(translationKey);
  }

  public selectAll(): void {
    this.selectedEntries = this.areAllSelected ? [] : this.entries;
  }

  public isSelected(entry: IEstablishmentWithAttachments): boolean {
    return !!this.selectedEntries.find(e => e.establishment.id === entry.establishment.id);
  }

  public select(entry: IEstablishmentWithAttachments): void {
    this.selectedEntries = this.isSelected(entry)
      ? this.selectedEntries.filter(e => e.establishment.id !== entry.establishment.id)
      : [...this.selectedEntries, entry];
  }

  public getEstablishmentStatus(entry: IEstablishmentWithAttachments): string {
    if (this.type === EstablishmentType.WithError) {
      return this.getErrorStatus(entry);
    }

    const isCovered = !!entry.currentAttachment;
    if (!isCovered) {
      return this.translatePipe.transform('front_contractPage_establishments_state_futureActivation');
    }

    return this.translatePipe.transform('front_contractPage_establishments_state_inProgress');
  }

  private getErrorStatus(entry: IEstablishmentWithAttachments): string {
    const wasCovered = !!entry.lastAttachment;
    const isCovered = !!entry.currentAttachment;
    const willBeCovered = !!entry.nextAttachment;

    if (isCovered && !entry.establishment.isActive) {
      return this.translatePipe.transform('front_contractPage_establishments_errorState_deleted');
    }

    if (!wasCovered && !willBeCovered) {
      return this.translatePipe.transform('front_contractPage_establishments_errorState_waitingFirstActivation');
    }

    if (willBeCovered && !wasCovered) {
      const until = this.datePipe.transform(entry.nextAttachment.start, 'dd/MM/YYYY');
      return this.translatePipe.transform('front_contractPage_establishments_errorState_notLinkedUntil', { until });
    }

    if (wasCovered && !willBeCovered) {
      const since = this.datePipe.transform(entry.lastAttachment.end, 'dd/MM/YYYY');
      return this.translatePipe.transform('front_contractPage_establishments_errorState_notLinkedSince', { since });
    }

    const from = this.datePipe.transform(entry.lastAttachment.end, 'dd/MM/YYYY');
    const to = this.datePipe.transform(entry.nextAttachment.start, 'dd/MM/YYYY');
    return this.translatePipe.transform('front_contractPage_establishments_errorState_notCoveredBetween', { from, to });
  }
}
