import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IDateRange } from '@cc/common/date';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { CountCode } from '@cc/domain/billing/counts';
import { LuModal } from '@lucca-front/ng/modal';
import { endOfMonth, isEqual, max, min, startOfMonth } from 'date-fns';
import { BehaviorSubject, combineLatest, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { debounceTime, filter, finalize, map, take, takeUntil } from 'rxjs/operators';

import { ContractsManageModalService } from '../../contracts-manage-modal.service';
import { CountsReplayModalComponent } from './components/counts-replay-modal/counts-replay-modal.component';
import { ICountContract } from './models/count-contract.interface';
import { ICountListEntry } from './models/count-list-entry.interface';
import { ICountsReplayModalData } from './models/counts-replay-modal-data.interface';
import { IDetailedCount } from './models/detailed-count.interface';
import { CountContractsDataService } from './services/count-contracts-data.service';
import { CountContractsListService } from './services/count-contracts-list.service';
import { CountContractsRestrictionsService } from './services/count-contracts-restrictions.service';

@Component({
  selector: 'cc-count-tab',
  templateUrl: './count-tab.component.html',
})
export class CountTabComponent implements OnInit, OnDestroy {
  public get contract$(): Observable<ICountContract> {
    return this.contractSubject$.asObservable();
  }

  public get countListEntries$(): Observable<ICountListEntry[]> {
    return this.countListEntriesSubject$.asObservable();
  }

  public get draftCounts$(): Observable<ICountListEntry[]> {
    return this.countListEntries$.pipe(
      map(entries => entries.filter(entry => !!entry?.count && entry.count.code === CountCode.Draft)),
    );
  }

  public get realCounts$(): Observable<ICountListEntry[]> {
    return this.countListEntries$.pipe(
      map(entries => entries.filter(entry => !!entry?.count && entry.count.code === CountCode.Count)),
    );
  }

  public get firstCountWithDetails$(): Observable<IDetailedCount> {
    return this.countListEntries$.pipe(this.toFirstCountWithDetails);
  }

  public get isLoading$(): Observable<boolean> {
    return this.isLoadingSubject$.asObservable();
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  public showDraftCounts = false;
  public countsSelected: IDetailedCount[] = [];
  public isDownloadFormDisplay = false;

  public deleteButtonState$: Subject<string> = new Subject<string>();
  public downloadButtonState$: Subject<string> = new Subject<string>();
  public deleteDraftButtonState$: Subject<string> = new Subject<string>();
  public chargeDraftButtonState$: Subject<string> = new Subject<string>();

  private contractSubject$: ReplaySubject<ICountContract> = new ReplaySubject(1);
  private countListEntriesSubject$: BehaviorSubject<ICountListEntry[]> = new BehaviorSubject([]);
  private isLoadingSubject$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  private destroy$: Subject<void> = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private contractsManageModalService: ContractsManageModalService,
    private countContractsListService: CountContractsListService,
    private countContractsService: CountContractsDataService,
    private restrictionsService: CountContractsRestrictionsService,
    private luModal: LuModal,
    private manageModalService: ContractsManageModalService,
  ) { }

  public ngOnInit(): void {
    this.refresh(this.contractId);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public charge(missingCountPeriod: Date): void {
    this.countContractsService.charge$(this.contractId, startOfMonth(missingCountPeriod), endOfMonth(missingCountPeriod))
      .pipe(this.toButtonState, debounceTime(300))
      .subscribe(state => {
        const entries = this.countListEntriesSubject$.value;
        const entry = entries.find(e => isEqual(e.month, missingCountPeriod));
        entry.buttonStateClass = state;

        this.countListEntriesSubject$.next(entries);

        if (entries.every(e => !e.buttonStateClass)) {
          this.refresh(this.contractId);
        }
      });
  }

  public delete(counts: IDetailedCount[]): void {
    this.countContractsService.deleteRange$(counts)
      .pipe(this.toButtonState, finalize(() => {
          this.refresh(this.contractId);
          this.countsSelected = [];
        }),
      )
      .subscribe(state => this.deleteButtonState$.next(state));
  }

  public download(period: IDateRange): void {
    this.countContractsService.download$(this.contractId, period)
      .pipe(this.toButtonState, finalize(() => this.toggleDownloadFormDisplay()))
      .subscribe(state => this.downloadButtonState$.next(state));
  }

  public chargeDraftCounts(): void {
    const countPeriodToCharge = new Date();
    this.countContractsService.charge$(this.contractId, startOfMonth(countPeriodToCharge), endOfMonth(countPeriodToCharge))
      .pipe(this.toButtonState, finalize(() => {
        this.refresh(this.contractId);
        this.toggleDraftCountsDisplay();
      }))
      .subscribe(state => this.chargeDraftButtonState$.next(state));
  }

  public deleteDraftCount(): void {
    this.countContractsService.deleteDraftCount$(this.contractId)
      .pipe(this.toButtonState, finalize(() => {
        this.refresh(this.contractId);
        this.showDraftCounts = false;
      }))
      .subscribe(state => this.deleteDraftButtonState$.next(state));
  }

  public canDeleteCount(): boolean {
    return this.restrictionsService.canDeleteAtLeastOneCount(this.countListEntriesSubject$.value);
  }

  public canChargeCount(): boolean {
    return this.restrictionsService.canChargeCount();
  }

  public openReplayModal(): void {
    const missingCountPeriods = this.countListEntriesSubject$.value.filter(c => !c.count).map(c => c.month);
    const selectableCountPeriods = this.restrictionsService.getAllCountEntriesSelectable(this.countListEntriesSubject$.value)
      .map(c => c.month);

    const periods = [...missingCountPeriods, ...selectableCountPeriods].map(period => new Date(period));

    const modalData: ICountsReplayModalData = {
      entries: this.countListEntriesSubject$.value,
      contractId: this.contractId,
      min: min(periods),
      max: max(periods),
    };

    const luModalRef = this.luModal.open(CountsReplayModalComponent, modalData);
    luModalRef.onClose.pipe(takeUntil(this.destroy$)).subscribe(() => this.refresh(this.contractId));
  }

  public updateCountsSelected(counts: IDetailedCount[]): void {
    this.countsSelected = counts;
  }

  public toggleDraftCountsDisplay(): void {
    this.showDraftCounts = !this.showDraftCounts;
  }

  public toggleDownloadFormDisplay(): void {
    this.isDownloadFormDisplay = !this.isDownloadFormDisplay;
  }

  private refresh(contractId: number): void {
    this.isLoadingSubject$.next(true);

    combineLatest([
      this.countContractsService.getDetailedCounts$(contractId),
      this.countContractsService.getCountContract$(contractId),
    ])
      .pipe(take(1), finalize(() => this.isLoadingSubject$.next(false)))
      .subscribe(
        ([counts, contract]) => {
          this.contractSubject$.next(contract);
          this.countListEntriesSubject$.next(this.countContractsListService.toCountListEntries(counts, contract));
        },
        err => this.manageModalService.close(),
      );
  }

  private get toButtonState(): UnaryFunction<Observable<void>, Observable<string>> {
    return pipe(
      take(1),
      toSubmissionState(),
      map(state => getButtonState(state)),
    );
  }

  private get toFirstCountWithDetails(): UnaryFunction<Observable<ICountListEntry[]>, Observable<IDetailedCount>> {
    return pipe(
      map(entries => entries.map(entry => entry.count)),
      map(counts => counts.filter(count => count?.hasDetails)),
      map(counts => counts.sort((a, b) => new Date(a?.countPeriod).getTime() - new Date(b?.countPeriod).getTime())),
      filter(counts => !!counts?.length),
      map(counts => counts[0]),
    );
  }
}
