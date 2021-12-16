import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { IEnvironment } from '@cc/domain/environments';
import { Observable, of, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { distinctUntilChanged, filter, finalize, map, share, shareReplay, startWith, switchMap, take, takeUntil } from 'rxjs/operators';

import { ContractsListService } from '../../../../services/contracts-list.service';
import { ValidationContextStoreService } from '../../validation-context-store.service';
import { CreationCause } from './constants/creation-cause.enum';
import { IContractEnvironmentDetailed } from './models/contract-environment-detailed.interface';
import { IEnvironmentDetailed } from './models/environment-detailed.interface';
import { IPreviousContractEnvironment } from './models/previous-contract-environment.interface';
import { ContractEnvironmentService } from './services/contract-environment.service';
import { ContractEnvironmentActionRestrictionsService } from './services/contract-environment-action-restrictions.service';
import { EnvironmentCreationCauseService } from './services/environment-creation-cause.service';

enum LinkingFormKey {
  Environment = 'environment',
  Cause = 'creationCause',
}

@Component({
  selector: 'cc-environment-tab',
  templateUrl: './environment-tab.component.html',
  styleUrls: ['./environment-tab.component.scss'],
})
export class EnvironmentTabComponent implements OnInit, OnDestroy {
  public contractEnvironment$: ReplaySubject<IContractEnvironmentDetailed>
    = new ReplaySubject<IContractEnvironmentDetailed>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public environmentLinked$: Observable<IEnvironment | null>;
  public environmentsSuggested$: Observable<IEnvironmentDetailed[]>;
  public canRemoveEnvironmentLinked$: Observable<boolean>;

  public unlinkButtonState$: Subject<string> = new Subject<string>();
  public linkButtonState$: Subject<string> = new Subject<string>();

  public previousContracts$: Observable<IPreviousContractEnvironment[]>;
  public arePreviousContractsLoading$: Subject<boolean> = new Subject<boolean>();

  public formGroup: FormGroup;
  public formGroupKey = LinkingFormKey;

  private get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private activatedRoute: ActivatedRoute,
    private contractsListService: ContractsListService,
    private contractEnvironmentService: ContractEnvironmentService,
    private creationCauseService: EnvironmentCreationCauseService,
    private contractEnvironmentActionRestrictionsService: ContractEnvironmentActionRestrictionsService,
    private contextStoreService: ValidationContextStoreService,
  ) {
    this.formGroup = new FormGroup({
      [LinkingFormKey.Environment]: new FormControl(null, Validators.required),
      [LinkingFormKey.Cause]: new FormControl(null, Validators.required),
    });
  }

  public ngOnInit(): void {
    this.refreshContractEnvironment();

    this.environmentLinked$ = this.contractEnvironment$
      .pipe(shareReplay(1), map(contract => contract.environment));

    this.canRemoveEnvironmentLinked$ = this.contextStoreService.activeEtsNumber$
      .pipe(
        map(count => this.contractEnvironmentActionRestrictionsService.canRemoveEnvironmentLinked(count)),
        startWith(false),
      );

    this.environmentsSuggested$ = this.contractEnvironment$
      .pipe(this.toEnvironmentsSuggested(), shareReplay(1));

    this.previousContracts$ = this.environmentSelected$.pipe(
      takeUntil(this.destroy$),
      switchMap((environment: IEnvironmentDetailed) => this.getPreviousContracts$(environment?.id)),
      share(),
      startWith([]),
    );

    this.previousContracts$
      .pipe(takeUntil(this.destroy$))
      .subscribe(contracts => {
        if (!contracts.length && !!this.formGroup.get(LinkingFormKey.Environment).value) {
          this.formGroup.get(LinkingFormKey.Cause).setValue(CreationCause.NewBooking);
        }
      });

    this.environmentSelected$
      .pipe(
        map(environment => !environment),
        takeUntil(this.destroy$),
        filter(isEnvironmentReset => !!isEnvironmentReset),
      )
      .subscribe(() => this.formGroup.get(LinkingFormKey.Cause).reset());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public unlinkEnvironment(): void {
    this.contractEnvironmentService.unlinkEnvironment$(this.contractId)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.refresh()),
      )
      .subscribe(buttonState => this.unlinkButtonState$.next(buttonState));
  }

  public linkEnvironment(): void {
    const environmentId = this.formGroup.value.environment.id;
    const creationCause = this.formGroup.value.creationCause;
    this.contractEnvironmentService.linkEnvironment$(this.contractId, environmentId, creationCause)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.formGroup.reset();
          this.refresh();
        }))
      .subscribe(buttonState => this.linkButtonState$.next(buttonState));
  }

  private refresh(): void {
    this.contractsListService.refresh();
    this.refreshContractEnvironment();
  }

  private refreshContractEnvironment(): void {
    this.isLoading$.next(true);

    const contractEnvironment$ = this.contractEnvironmentService.getContractEnvironment$(this.contractId)
      .pipe(take(1), share(), finalize(() => this.isLoading$.next(false)));

    contractEnvironment$.pipe(take(1))
      .subscribe(c => this.contractEnvironment$.next(c));
  }

  private toEnvironmentsSuggested(): UnaryFunction<Observable<IContractEnvironmentDetailed>, Observable<IEnvironmentDetailed[]>> {
    return pipe(
      filter(contract => !contract.environment),
      switchMap(contract => this.contractEnvironmentService.getSuggestedEnvironments$(contract.clientId)),
      startWith([]),
    );
  }

  private getPreviousContracts$(environmentId: number): Observable<IPreviousContractEnvironment[]> {
    if (!environmentId) {
      return of([]);
    }

    this.arePreviousContractsLoading$.next(true);

    return this.contractEnvironment$.pipe(
      switchMap(c => this.creationCauseService.getPreviousContracts$(environmentId, c)),
      finalize(() => this.arePreviousContractsLoading$.next(false)),
      take(1),
    );
  }

  private get environmentSelected$(): Observable<IEnvironmentDetailed> {
    return this.formGroup.get(LinkingFormKey.Environment).valueChanges
      .pipe(distinctUntilChanged());
  }
}
