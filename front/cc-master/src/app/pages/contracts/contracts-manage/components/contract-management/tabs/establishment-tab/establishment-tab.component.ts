import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { ICount } from '@cc/domain/billing/counts';
import { TimelineCountsService } from '@cc/domain/billing/counts/timeline-counts-service';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, finalize, map, take, takeUntil } from 'rxjs/operators';

import { ValidationContextStoreService } from '../../validation-context-store.service';
import { EstablishmentFilterKey, IEstablishmentFilterForm } from './components/establishment-filters/establishment-filters.component';
import { establishmentDocUrl } from './constants/establishment-doc-url.const';
import { IEstablishmentActionsContext } from './models/establishment-actions-context.interface';
import { IEstablishmentContract } from './models/establishment-contract.interface';
import { IListEntry, LifecycleStep, ListEntryType } from './models/establishment-list-entry.interface';
import { AttachmentsActionRestrictionsService } from './services/attachments-action-restrictions.service';
import { EstablishmentContractDataService } from './services/establishment-contract-data.service';
import { EstablishmentListActionsService } from './services/establishment-list-actions.service';
import { EstablishmentListEntriesService } from './services/establishment-list-entries.service';
import { EstablishmentsDataService } from './services/establishments-data.service';

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
  public get type(): ListEntryType {
    return this.filters.value[EstablishmentFilterKey.Type];
  }

  public filters: FormControl = new FormControl({
    [EstablishmentFilterKey.Type]: ListEntryType.LinkedToThisContract,
    [EstablishmentFilterKey.ShowFinishedAttachments]: false,
  });

  public contract$: ReplaySubject<IEstablishmentContract> = new ReplaySubject<IEstablishmentContract>(1);
  public context$: ReplaySubject<IEstablishmentActionsContext> = new ReplaySubject<IEstablishmentActionsContext>(1);

  public isLoading$: ReplaySubject<boolean> = new ReplaySubject(1);
  public synchronizeButtonClass$: ReplaySubject<string> = new ReplaySubject(1);

  public allEntriesExceptWithError$: ReplaySubject<IListEntry[]> = new ReplaySubject(1);
  public entriesWithError$: ReplaySubject<IListEntry[]> = new ReplaySubject(1);
  private entries$: ReplaySubject<IListEntry[]> = new ReplaySubject(1);

  private destroy$: Subject<void> = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private contractService: EstablishmentContractDataService,
    private establishmentsService: EstablishmentsDataService,
    private actionsService: EstablishmentListActionsService,
    private contextStoreService: ValidationContextStoreService,
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

    this.entries$
      .pipe(takeUntil(this.destroy$), map(entries => entries.filter(e => e.type !== ListEntryType.Error)))
      .subscribe(this.allEntriesExceptWithError$);

    this.entries$
      .pipe(takeUntil(this.destroy$), map(entries => entries.filter(e => e.type === ListEntryType.Error)))
      .subscribe(this.entriesWithError$);

    this.allEntriesExceptWithError$
      .pipe(
        takeUntil(this.destroy$),
        map(entries => this.hasFinishedEntriesByType(entries, this.filters.value.type)),
        filter(hasFinishedEntries => !hasFinishedEntries),
      )
      .subscribe(() => this.updateShowFinishedAttachments(false));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public getFilteredEntries(allEntriesExceptWithError: IListEntry[], filters: IEstablishmentFilterForm): IListEntry[] {
    const entries = allEntriesExceptWithError.filter(e => e.type === filters?.type);
    const sortedEntries = entries.sort((a, b) => new Date(a.attachment?.start).getTime() - new Date(b.attachment?.start).getTime());

    const [finishedEntries, others] = [
      sortedEntries.filter(e => e.lifecycleStep === LifecycleStep.Finished),
      sortedEntries.filter(e => e.lifecycleStep !== LifecycleStep.Finished),
    ];

    return filters.showFinishedAttachments ? [...finishedEntries, ...others] : others;
  }

  public hasFinishedEntriesByType(allEntriesExceptWithError: IListEntry[], type: ListEntryType): boolean {
    return allEntriesExceptWithError.some(e => e.lifecycleStep === LifecycleStep.Finished && e.type === type);
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
      this.listService.getListEntries$(contract)
        .pipe(take(1), finalize(() => this.isLoading$.next(false)))
        .subscribe(establishments => {
          this.contract$.next(contract);
          this.entries$.next(establishments);
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
        this.entries$.next(establishments);
        this.context$.next(this.toContext(contract, realCounts, establishments));
      });
  }

  private toContext(contract: IEstablishmentContract, realCounts: ICount[], allEntries: IListEntry[]): IEstablishmentActionsContext {
    const lastCountPeriod = TimelineCountsService.getLastCountPeriod(realCounts);
    return { contract, realCounts, lastCountPeriod, allEntries };
  }

  private updateShowFinishedAttachments(showFinishedAttachments: false): void {
    this.filters.patchValue({
      ...this.filters.value,
      [EstablishmentFilterKey.ShowFinishedAttachments]: showFinishedAttachments,
    });
  }
}
