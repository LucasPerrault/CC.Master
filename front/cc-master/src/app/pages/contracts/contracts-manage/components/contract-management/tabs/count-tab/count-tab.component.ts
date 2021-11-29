import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { CountCode } from '@cc/domain/billing/counts';
import { LuModal } from '@lucca-front/ng/modal';
import { endOfMonth, isEqual, max, min, startOfMonth } from 'date-fns';
import { BehaviorSubject, combineLatest, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { debounceTime, finalize, map, take, takeUntil } from 'rxjs/operators';

import { ContractManagementService } from '../../contract-management.service';
import { CountsDetailDownloadModalComponent } from './components/counts-detail-download-modal/counts-detail-download-modal.component';
import { ICountsDetailDownloadModalData } from './components/counts-detail-download-modal/counts-detail-download-modal-data.interface';
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
  styleUrls: ['./count-tab.component.scss'],
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

  public get hasDetails$(): Observable<boolean> {
    return this.countListEntries$.pipe(map(entries => entries.some(e => e.count?.hasDetails)));
  }

  public get isLoading$(): Observable<boolean> {
    return this.isLoadingSubject$.asObservable();
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  public showDraftCounts = false;
  public countsSelected: IDetailedCount[] = [];

  public deleteButtonState$: Subject<string> = new Subject<string>();
  public deleteDraftButtonState$: Subject<string> = new Subject<string>();
  public chargeDraftButtonState$: Subject<string> = new Subject<string>();

  private contractSubject$: BehaviorSubject<ICountContract> = new BehaviorSubject(null);
  private countListEntriesSubject$: BehaviorSubject<ICountListEntry[]> = new BehaviorSubject([]);
  private isLoadingSubject$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  private destroy$: Subject<void> = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private contractsManageModalService: ContractManagementService,
    private countContractsListService: CountContractsListService,
    private countContractsService: CountContractsDataService,
    private restrictionsService: CountContractsRestrictionsService,
    private luModal: LuModal,
    private manageModalService: ContractManagementService,
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

  public openDownloadModal(): void {
    const firstCountPeriod = this.getFirstCount(this.countListEntriesSubject$.value)?.countPeriod;
    const closeOn = this.contractSubject$.value.closeOn;
    const data: ICountsDetailDownloadModalData = {
      contractId: this.contractId,
      firstCountPeriod: !!firstCountPeriod ? new Date(firstCountPeriod) : null,
      contractCloseOn: !!closeOn ? new Date(closeOn) : null,
    };
    this.luModal.open(CountsDetailDownloadModalComponent, data);
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
          const entries = this.countContractsListService.toCountListEntries(counts, contract);
          this.countListEntriesSubject$.next(entries);
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

  private getFirstCount(entries: ICountListEntry[]): IDetailedCount {
    const counts = entries.map(e => e.count);
    const countsWithDetails = counts.filter(count => count?.hasDetails);
    const sortedAscCounts = countsWithDetails.sort((a, b) => new Date(a?.countPeriod).getTime() - new Date(b?.countPeriod).getTime());
    return !!sortedAscCounts?.length ? sortedAscCounts[0] : null;
  }
}
