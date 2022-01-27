import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RightsService } from '@cc/aspects/rights';
import { TranslatePipe } from '@cc/aspects/translate';
import { getButtonState, SubmissionState, toSubmissionState } from '@cc/common/forms';
import { INavigationTab, NavigationPath } from '@cc/common/navigation';
import { ToastsService, ToastType } from '@cc/common/toasts';
import { IContract } from '@cc/domain/billing/contracts';
import { BehaviorSubject, combineLatest, from, Observable, ReplaySubject, Subject } from 'rxjs';
import { catchError, finalize, map, startWith, take, takeUntil } from 'rxjs/operators';

import { ContractsModalTabPath } from './constants/contracts-modal-tab-path.enum';
import { contractsModalTabs } from './constants/contracts-modal-tabs.const';
import { ContractManagementService } from './contract-management.service';
import { ContractManagementDataService } from './contract-management-data.service';
import { ValidationContextStoreService } from './validation-context-store.service';
import { ValidationRestrictionsService } from './validation-restrictions.service';

@Component({
  selector: 'cc-contracts-manage-modal',
  templateUrl: './contract-management.component.html',
  styleUrls: ['./contract-management.component.scss'],
})
export class ContractManagementComponent implements OnInit, OnDestroy {
  public title: string;
  public isContractLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public isNotFound$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public deleteButtonClass$: Subject<string> = new Subject<string>();

  public get isLoading$(): Observable<boolean> {
    return combineLatest([this.isContractLoading$, this.contextStoreService.isLoading$])
      .pipe(map(loadings => loadings.every(isLoading => !!isLoading)));
  }

  public get canDeleteContract$(): Observable<boolean> {
    return this.contextStoreService.context$
      .pipe(map(c => this.restrictionsService.canDeleteContracts(c)), startWith(false));
  }

  private readonly contractId: number;
  private destroy$: Subject<void> = new Subject<void>();

  public get tabs(): INavigationTab[] {
    return contractsModalTabs.filter(tab =>
      this.rightsService.hasOperationsByRestrictionMode(tab.restriction.operations, tab.restriction.mode),
    );
  }

  constructor(
    private rightsService: RightsService,
    private activatedRoute: ActivatedRoute,
    private dataService: ContractManagementDataService,
    private translatePipe: TranslatePipe,
    private router: Router,
    private contractsManageModalService: ContractManagementService,
    private contextStoreService: ValidationContextStoreService,
    private restrictionsService: ValidationRestrictionsService,
    private toastsService: ToastsService,
  ) {
    this.contractId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'), 10);
  }

  public ngOnInit(): void {
    if (this.restrictionsService.hasRightsToReadValidationContext) {
      this.contextStoreService.init(this.contractId);
    }

    this.updateContractTitle();

    this.contractsManageModalService.onRefresh$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.updateContractTitle());

    this.contractsManageModalService.onClose$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.redirectToContracts());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public delete(): void {
    this.dataService.deleteContract$(this.contractId)
      .pipe(take(1), toSubmissionState())
      .subscribe(state => {
        this.deleteButtonClass$.next(getButtonState(state));
        this.notifyAndRedirect(state);
      });
  }

  public redirectToContracts(): void {
    const route = `${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }`;
    this.router.navigate([route], {
      queryParamsHandling: 'preserve',
    });
  }

  private setTitle(contractName: string): void {
    this.title = !!contractName
      ? `${ this.translatePipe.transform('front_contractPage_modalTitle') } ${ contractName }`
      : '';
  }

  private updateContractTitle(): void {
    this.isContractLoading$.next(true);

    this.dataService.getContractById$(this.contractId)
      .pipe(
        takeUntil(this.destroy$),
        finalize(() => this.isContractLoading$.next(false)),
        catchError(() => from(this.redirectToNotFoundPage())),
      )
      .subscribe(contract => this.setTitle(contract?.name));
  }

  private redirectToNotFoundPage(): Promise<IContract> {
    this.isNotFound$.next(true);

    const route = [ContractsModalTabPath.NotFound];
    return this.router
      .navigate(route, { queryParamsHandling: 'preserve', relativeTo: this.activatedRoute })
      .then(() => null);
  }

  private notifyAndRedirect(state: SubmissionState): void {
    if (state !== SubmissionState.Success) {
      return;
    }

    this.redirectToContracts();
    this.toastsService.addToast({
      type: ToastType.Success,
      message: this.translatePipe.transform('front_contractPage_deletion_successMessage', { id: this.contractId }),
    });
  }
}
