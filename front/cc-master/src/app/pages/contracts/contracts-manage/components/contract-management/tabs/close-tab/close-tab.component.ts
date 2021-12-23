import { DatePipe } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { combineLatest, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { finalize, map, share, take, takeUntil } from 'rxjs/operators';

import { ContractsListService } from '../../../../services/contracts-list.service';
import { IClosureFormValidationContext, IContextAttachment } from './models/closure-form-validation-context.interface';
import { IContractClosureDetailed } from './models/contract-closure-detailed.interface';
import { IContractClosureForm } from './models/contract-closure-form.interface';
import { CloseContractService } from './services/close-contract.service';
import { CloseContractFormService } from './services/close-contract-form.service';

@Component({
  selector: 'cc-close-tab',
  templateUrl: './close-tab.component.html',
  styleUrls: ['./close-tab.component.scss'],
})
export class CloseTabComponent implements OnInit, OnDestroy {
  public contractClosureDetailed$: ReplaySubject<IContractClosureDetailed>
    = new ReplaySubject<IContractClosureDetailed>(1);
  public mostRecentAttachment$ = new ReplaySubject<IContextAttachment | null>(1);
  public lastCountPeriod$: ReplaySubject<Date | null>
    = new ReplaySubject<Date | null>(1);
  public closureFormValidationContext$: Observable<IClosureFormValidationContext>;
  public maxContractClosedDate$: Observable<Date>;
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public isContractClosed$: Observable<boolean>;
  public cancelButtonState$: Subject<string> = new Subject<string>();
  public closeButtonState$: Subject<string> = new Subject<string>();

  public get canReadValidationContext(): boolean {
    return this.closeContractFormService.canReadValidationContext;
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
    private closeContractFormService: CloseContractFormService,
    private contractsListService: ContractsListService,
  ) {}

  public ngOnInit(): void {
    this.refreshContractClosureDetailed();

    const closureConditions$ = [this.contractClosureDetailed$, this.lastCountPeriod$, this.mostRecentAttachment$];
    this.closureFormValidationContext$ = combineLatest(closureConditions$).pipe(
      takeUntil(this.destroy$),
      this.toClosureFormValidationContext(),
    );

    this.maxContractClosedDate$ = this.closureFormValidationContext$.pipe(
      takeUntil(this.destroy$),
      map(context => this.closeContractFormService.getMaxContractClosedDate(context)),
    );

    this.isContractClosed$ = this.contractClosureDetailed$
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
    this.isLoading$.next(true);

    if (this.closeContractFormService.canReadValidationContext) {
      this.closeContractFormService.getMostRecentAttachment$(this.contractId)
        .pipe(take(1))
        .subscribe(e => this.mostRecentAttachment$.next(e));

      this.closeContractFormService.getLastCountPeriod$()
        .pipe(take(1))
        .subscribe(p => this.lastCountPeriod$.next(p));
    }

    const contractClosureDetailed$ = this.closeContractService.getContractClosureDetailed$(this.contractId)
      .pipe(take(1), share());

    contractClosureDetailed$
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(c => this.contractClosureDetailed$.next(c));
  }

  private toClosureFormValidationContext(
  ): UnaryFunction<Observable<[IContractClosureDetailed, Date, IContextAttachment]>, Observable<IClosureFormValidationContext>> {
    return pipe(
      map(([contract, lastCountPeriod, mostRecentAttachment]) => ({
        theoreticalStartOn: new Date(contract.theoricalStartOn),
        lastCountPeriod: !!lastCountPeriod ? new Date(lastCountPeriod) : null,
        mostRecentAttachment,
      })),
    );
  }
}
