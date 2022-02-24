import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take, takeUntil } from 'rxjs/operators';

import { ContractsListService } from '../../../../services/contracts-list.service';
import { ICloseContractMinDateReason } from './models/close-contract-min-date-reason.interface';
import { IContractClosureDetailed } from './models/contract-closure-detailed.interface';
import { IContractClosureForm } from './models/contract-closure-form.interface';
import { CloseContractService } from './services/close-contract.service';
import { CloseContractDataService } from './services/close-contract-data.service';
import { CloseContractRestrictionsService } from './services/close-contract-restrictions.service';

@Component({
  selector: 'cc-close-tab',
  templateUrl: './close-tab.component.html',
  styleUrls: ['./close-tab.component.scss'],
})
export class CloseTabComponent implements OnInit, OnDestroy {
  public contract$ = new ReplaySubject<IContractClosureDetailed>(1);
  public closeContractMinDateReason$ = new ReplaySubject<ICloseContractMinDateReason>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public isContractClosed$: Observable<boolean>;
  public cancelButtonState$: Subject<string> = new Subject<string>();
  public closeButtonState$: Subject<string> = new Subject<string>();

  public get canReadValidationContext(): boolean {
    return this.restrictionsService.canReadValidationContext;
  }

  private destroy$: Subject<void> = new Subject<void>();

  private get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  constructor(
    private datePipe: DatePipe,
    private translatePipe: TranslatePipe,
    private activatedRoute: ActivatedRoute,
    private closeContractService: CloseContractService,
    private dataService: CloseContractDataService,
    private contractsListService: ContractsListService,
    private restrictionsService: CloseContractRestrictionsService,
  ) {}

  public ngOnInit(): void {
    this.refreshContractClosureDetailed();

    this.isContractClosed$ = this.contract$
      .pipe(takeUntil(this.destroy$), map(contract => !!contract.closeReason && !!contract.closeOn));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public closeContract(form: IContractClosureForm): void {
    this.closeContractService.closeContract$(this.contractId, form.closeOn, form.closeReason)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.contractsListService.refresh();
          this.refreshContractClosureDetailed();
        }))
      .subscribe(state => this.closeButtonState$.next(state));
  }

  public cancelClosure(): void {
    this.closeContractService.cancelContractClosure$(this.contractId)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.contractsListService.refresh();
          this.refreshContractClosureDetailed();
        }))
      .subscribe(state => this.cancelButtonState$.next(state));
  }

  private refreshContractClosureDetailed(): void {
    if (!this.restrictionsService.canReadValidationContext) {
      return;
    }

    this.isLoading$.next(true);

    const mostRecentAttachment$ = this.dataService.getMostRecentAttachment$(this.contractId);
    const lastCountPeriod$ = this.dataService.getLastCountPeriod$();
    const contract$ = this.closeContractService.getContractClosureDetailed$(this.contractId);

    combineLatest([contract$, lastCountPeriod$, mostRecentAttachment$])
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(([contract, lastCountPeriod, mostRecentAttachment]) => {
        const contractStartDate = new Date(contract.theoricalStartOn);
        const lastCountDate = new Date(lastCountPeriod);
        const minDateReason = this.restrictionsService.getCloseMinDateReason(contractStartDate, lastCountDate, mostRecentAttachment);
        this.closeContractMinDateReason$.next(minDateReason);
        this.contract$.next(contract);
      });
  }
}
