import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Operation, RightsService } from '@cc/aspects/rights';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { IContractForm } from '@cc/domain/billing/contracts';
import { DistributorsService } from '@cc/domain/billing/distributors';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, startWith, switchMap, take } from 'rxjs/operators';

import { ContractsListService } from '../../../../services/contracts-list.service';
import { ContractManagementService } from '../../contract-management.service';
import { IValidationContext } from '../../validation-context-store.data';
import { ValidationContextStoreService } from '../../validation-context-store.service';
import { IContractDetailed } from './models/contract-detailed.interface';
import { ContractActionRestrictionsService } from './services/contract-action-restrictions.service.';
import { ContractTabService } from './services/contract-tab.service';

@Component({
  selector: 'cc-contract-tab',
  templateUrl: './contract-tab.component.html',
  styleUrls: ['./contract-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContractTabComponent implements OnInit {

  public contractForm: FormControl = new FormControl();
  public validationContext$: ReplaySubject<IValidationContext> = new ReplaySubject(1);
  public formInformation$: ReplaySubject<IContractFormInformation> = new ReplaySubject(1);
  public showDeletionCallout = true;

  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public editButtonState$: Subject<string> = new Subject<string>();
  public deleteState$: Subject<string> = new Subject<string>();

  public get canDeleteContract$(): Observable<boolean> {
    return this.validationContext$.pipe(
      map(c => this.contractFormValidationService.canDeleteContracts(c)),
      startWith(false),
    );
  }

  public get canEditContract(): boolean {
    return this.rightsService.hasOperation(Operation.EditContracts);
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  private detailedContract$: ReplaySubject<IContractDetailed> = new ReplaySubject<IContractDetailed>(1);

  constructor(
    private rightsService: RightsService,
    private activatedRoute: ActivatedRoute,
    private contractTabService: ContractTabService,
    private contractFormValidationService: ContractActionRestrictionsService,
    private contractsManageModalService: ContractManagementService,
    private contractsListService: ContractsListService,
    private contextStoreService: ValidationContextStoreService,
  ) {}

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.contextStoreService.context$
      .pipe(take(1))
      .subscribe(contract => this.contractForm.setValue(this.toContractForm(contract)));

    this.detailedContract$
      .pipe(take(1), switchMap(contract => this.toFormInformation$(contract)))
      .subscribe(this.formInformation$);
  }

  public edit(): void {
    this.contractTabService.updateContract$(this.contractId, this.contractForm.value)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.contractsManageModalService.refresh();
          this.contractsListService.refresh();
        }),
      )
      .subscribe(buttonState => this.editButtonState$.next(buttonState));
  }

  public delete(): void {
    this.contractTabService.deleteContract$(this.contractId)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.contractsListService.refresh();
          this.contractsManageModalService.close();
        }),
      )
      .subscribe(state => this.deleteState$.next(state));
  }

  public close(): void {
    this.contractsManageModalService.close();
  }

  public hideDeletionCallout(): void {
    this.showDeletionCallout = false;
  }

  private toContractForm(contractDetailed: IContractDetailed): IContractForm {
    return ({
      billingMonth: contractDetailed.billingMonth,
      distributor: contractDetailed.distributor,
      client: contractDetailed.client,
      offer: contractDetailed.offer,
      product: contractDetailed.product,
      theoreticalDraftCount: contractDetailed.unityNumberTheorical,
      clientRebate: {
        count: contractDetailed.clientRebate,
        endAt: !!contractDetailed.endClientRebateOn ? new Date(contractDetailed.endClientRebateOn) : null,
      },
      theoreticalMonthRebate: contractDetailed.nbMonthTheorical,
      theoreticalStartOn: new Date(contractDetailed.theoricalStartOn),
      minimalBillingPercentage: contractDetailed.minimalBillingPercentage,
      comment: contractDetailed.comment ?? '',
    });
  }

  private toFormInformation$(contract: IContractDetailed): Observable<IContractFormInformation> {
    return this.distributorsService.getActiveRebate$(contract.distributor?.id, contract.product?.id).pipe(
        map(distributorRebate => ({
          client: contract.client,
          distributorRebate,
        })),
    );
  }
}
