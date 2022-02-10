import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { IContractForm } from '@cc/domain/billing/contracts';
import { DistributorsService } from '@cc/domain/billing/distributors';
import { ILuPopupRef, LuPopup } from '@lucca-front/ng/popup';
import { isEqual as isDateEqual } from 'date-fns';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, finalize, map, take, takeUntil } from 'rxjs/operators';

import { ContractsListService } from '../../../../services/contracts-list.service';
import { ContractManagementService } from '../../contract-management.service';
import { IValidationContext } from '../../validation-context-store.data';
import { ValidationContextStoreService } from '../../validation-context-store.service';
import { ValidationRestrictionsService } from '../../validation-restrictions.service';
import {
  ContractLeavingConfirmationPopupComponent,
} from './components/contract-leaving-confirmation-popup/contract-leaving-confirmation-popup.component';
import { IContractDetailed } from './models/contract-detailed.interface';
import { IContractFormInformation } from './models/contract-form-information.interface';
import { ContractTabService } from './services/contract-tab.service';

@Component({
  selector: 'cc-contract-tab',
  templateUrl: './contract-tab.component.html',
  styleUrls: ['./contract-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContractTabComponent implements OnInit, OnDestroy {

  public contractForm: FormControl = new FormControl();
  public validationContext$: ReplaySubject<IValidationContext> = new ReplaySubject(1);
  public formInformation$: ReplaySubject<IContractFormInformation> = new ReplaySubject(1);

  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public editButtonState$: Subject<string> = new Subject<string>();
  public isClosePopupConfirmed$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public get canEditContract$(): Observable<boolean> {
    return this.validationContext$.pipe(map(context => this.restrictionsService.canEditContract(context)));
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  private savedForm$: BehaviorSubject<IContractForm> = new BehaviorSubject<IContractForm>(null);
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private activatedRoute: ActivatedRoute,
    private contractTabService: ContractTabService,
    private contractsManageModalService: ContractManagementService,
    private contractsListService: ContractsListService,
    private contextStoreService: ValidationContextStoreService,
    private restrictionsService: ValidationRestrictionsService,
    private distributorsService: DistributorsService,
    private luPopup: LuPopup,
  ) {}

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.contextStoreService.context$
      .pipe(take(1))
      .subscribe(context => this.validationContext$.next(context));

    this.contractTabService.getContractDetailed$(this.contractId)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(contract => {
        const form = this.toContractForm(contract);
        this.contractForm.setValue(form);
        this.savedForm$.next(form);
        this.setFormInformation(contract);
      });

    this.isClosePopupConfirmed$
      .pipe(takeUntil(this.destroy$), filter(isConfirmed => isConfirmed))
      .subscribe(() => this.contractsManageModalService.close());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
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
      .subscribe(buttonState => {
        this.editButtonState$.next(buttonState);
        this.savedForm$.next(this.contractForm.value);
      });
  }

  public close(): void {
    if (!this.hasFormChanged()) {
      this.contractsManageModalService.close();
      return;
    }

    const popupRef = this.luPopup.open(ContractLeavingConfirmationPopupComponent);
    popupRef.onClose.pipe(take(1))
      .subscribe(this.isClosePopupConfirmed$);
  }

  public openCloseConfirmationPopup(): ILuPopupRef<ContractLeavingConfirmationPopupComponent> {
    return this.luPopup.open(ContractLeavingConfirmationPopupComponent);
  }

  public hasFormChanged(): boolean {
    const isInitialized = !!this.savedForm$.value && !!this.contractForm.value;
    return isInitialized && !this.isEqual(this.savedForm$.value, this.contractForm.value);
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

  private isEqual(contract: IContractForm, form: IContractForm): boolean {
    return contract.distributor.id === form.distributor.id
      && contract.client.id === form.client.id
      && contract.offer.id === form.offer.id
      && contract.product.id === form.product.id
      && contract.billingMonth === form.billingMonth
      && isDateEqual(new Date(contract.theoreticalStartOn), new Date(form.theoreticalStartOn))
      && contract.clientRebate.count === form.clientRebate.count
      && isDateEqual(new Date(contract.clientRebate.endAt), new Date(form.clientRebate.endAt))
      && contract.theoreticalMonthRebate === form.theoreticalMonthRebate
      && contract.minimalBillingPercentage === form.minimalBillingPercentage
      && contract.theoreticalDraftCount === form.theoreticalDraftCount
      && (contract.comment ?? '') === form.comment;
  }

  private setFormInformation(contract: IContractDetailed): void {
    this.toFormInformation$(contract)
      .pipe(take(1))
      .subscribe(this.formInformation$);
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
