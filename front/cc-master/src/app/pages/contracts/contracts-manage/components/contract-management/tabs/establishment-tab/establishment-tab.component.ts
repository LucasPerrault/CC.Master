import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { ICount } from '@cc/domain/billing/counts';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take, takeUntil } from 'rxjs/operators';

import { ValidationContextStoreService } from '../../validation-context-store.service';
import { establishmentDocUrl } from './constants/establishment-doc-url.const';
import { IEstablishmentContract } from './models/establishment-contract.interface';
import { IEstablishmentWithAttachments } from './models/establishment-with-attachments.interface';
import { IEstablishmentsWithAttachmentsByType } from './models/establishments-by-type.interface';
import { AttachmentsActionRestrictionsService } from './services/attachments-action-restrictions.service';
import { IListEntry, ListEntryType } from './models/establishment-list-entry.interface';
import { EstablishmentContractDataService } from './services/establishment-contract-data.service';
import { EstablishmentListActionsService } from './services/establishment-list-actions.service';
import { EstablishmentListEntriesService } from './services/establishment-list-entries.service';
import { EstablishmentTypeService } from './services/establishment-type.service';
import { EstablishmentsDataService } from './services/establishments-data.service';
import { IEstablishmentActionsContext } from './models/establishment-actions-context.interface';
import { TimelineCountsService } from '@cc/domain/billing/counts/timeline-counts-service';

@Component({
  selector: 'cc-establishment-tab',
  templateUrl: './establishment-tab.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./establishment-tab.component.scss'],
})
export class EstablishmentTabComponent implements OnInit, OnDestroy {

  public get hasEnvironment$(): Observable<boolean> {
    return this.contract$.pipe(map(contract => !!contract.environmentId));
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  public listEntryType = ListEntryType;
  public typeFilter: FormControl = new FormControl(ListEntryType.LinkedToThisContract);

  public entries$: ReplaySubject<IListEntry[]> = new ReplaySubject<IListEntry[]>(1);
  public contract$: ReplaySubject<IEstablishmentContract> = new ReplaySubject<IEstablishmentContract>(1);
  public context$: ReplaySubject<IEstablishmentActionsContext> = new ReplaySubject<IEstablishmentActionsContext>(1);

  public isLoading$: ReplaySubject<boolean> = new ReplaySubject(1);
  public synchronizeButtonClass$: ReplaySubject<string> = new ReplaySubject(1);

  private destroy$: Subject<void> = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private contractService: EstablishmentContractDataService,
    private establishmentsService: EstablishmentsDataService,
    private actionsService: EstablishmentListActionsService,
    private contextStoreService: ValidationContextStoreService,
    private typeService: EstablishmentTypeService,
    private listService: EstablishmentListEntriesService,
    private restrictionsService: AttachmentsActionRestrictionsService,
  ) {
  }

  public ngOnInit(): void {
    this.refresh();

    this.actionsService.refreshList$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.refresh();
        this.contextStoreService.refreshAll(this.contractId);
      });
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getTotalCount(entries: IListEntry[], isErrorSection: boolean): number {
    const errorEntriesCount = EstablishmentTypeService.getEntriesByType(entries, ListEntryType.Error)?.length ?? 0;
    return isErrorSection ? errorEntriesCount : entries.length - errorEntriesCount;
  }

  public getSortedEntriesByType(allEntries: IListEntry[], type: ListEntryType): IListEntry[] {
    const entries = EstablishmentTypeService.getEntriesByType(allEntries, type);
    if (type !== ListEntryType.Error) {
      return entries.sort((a, b) =>
        new Date(a.attachment?.start).getTime() - new Date(b.attachment?.start).getTime());
    }
    return entries;
  }

  public openEstablishmentsDoc(): void {
    window.open(establishmentDocUrl);
  }

  public synchronize(environmentId: number): void {
    this.establishmentsService.synchronize(environmentId)
      .pipe(
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.refresh();
          this.contextStoreService.refreshAll(this.contractId);
        }),
      )
      .subscribe(buttonClass => this.synchronizeButtonClass$.next(buttonClass));
  }

  private refresh(): void {
    this.isLoading$.next(true);

    this.contractService.getContract$(this.contractId)
      .pipe(take(1))
      .subscribe(contract => this.set(contract));
  }

  private set(contract: IEstablishmentContract): void {
    if (!this.restrictionsService.canReadValidationContext) {
      this.getEstablishmentsByType$(contract)
        .pipe(take(1), finalize(() => this.isLoading$.next(false)))
        .subscribe(establishments => {
          this.contract$.next(contract);
          this.establishments$.next(establishments);
        });

      return;
    }

    combineLatest([
      this.contextStoreService.realCounts$,
      this.listService.getListEntries$(contract),
    ])
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(([realCounts, establishments]) => {
        this.contract$.next(contract);
        this.context$.next(this.toContext(contract, realCounts));
        this.entries$.next(establishments);
      });
  }

  private toContext(contract: IEstablishmentContract, realCounts: ICount[]): IEstablishmentActionsContext {
    const lastCountPeriod = TimelineCountsService.getLastCountPeriod(realCounts);
    return { contract, realCounts, lastCountPeriod };
  }
}
