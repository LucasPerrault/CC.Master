import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RightsService } from '@cc/aspects/rights';
import { TranslatePipe } from '@cc/aspects/translate';
import { INavigationTab, NavigationPath } from '@cc/common/navigation';
import { IContract } from '@cc/domain/billing/contracts';
import { BehaviorSubject, combineLatest, from, Observable, ReplaySubject, Subject } from 'rxjs';
import { catchError, finalize, map, takeUntil } from 'rxjs/operators';

import { ContractsModalTabPath } from './constants/contracts-modal-tab-path.enum';
import { contractsModalTabs } from './constants/contracts-modal-tabs.const';
import { ContractManagementService } from './contract-management.service';
import { ContractManagementDataService } from './contract-management-data.service';
import { ValidationContextStoreService } from './validation-context-store.service';

@Component({
  selector: 'cc-contracts-manage-modal',
  templateUrl: './contract-management.component.html',
  styleUrls: ['./contract-management.component.scss'],
})
export class ContractManagementComponent implements OnInit, OnDestroy {
  public title: string;
  public isContractLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public isNotFound$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public get isLoading$(): Observable<boolean> {
    return combineLatest([this.isContractLoading$, this.contextStoreService.isLoading$])
      .pipe(map(loadings => loadings.every(isLoading => !!isLoading)));
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
    private manageModalDataService: ContractManagementDataService,
    private translatePipe: TranslatePipe,
    private router: Router,
    private contractsManageModalService: ContractManagementService,
    private contextStoreService: ValidationContextStoreService,
  ) {
    this.contractId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'), 10);
  }

  public ngOnInit(): void {
    this.contextStoreService.init(this.contractId);

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

    this.manageModalDataService.getContractById$(this.contractId)
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
}
