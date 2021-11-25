import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Operation, RightsService } from '@cc/aspects/rights';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { IContractForm } from '@cc/domain/billing/contracts';
import { ILuPopupRef, LuPopup } from '@lucca-front/ng/popup';
import { isEqual as isDateEqual } from 'date-fns';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { filter, finalize, map, take, takeUntil } from 'rxjs/operators';

import { ContractsListService } from '../../../../services/contracts-list.service';
import { ContractManagementService } from '../../contract-management.service';
import { IValidationContext } from '../../validation-context-store.data';
import { ValidationContextStoreService } from '../../validation-context-store.service';
import { ContractLeavingConfirmationPopupComponent } from './components/contract-leaving-confirmation-popup/contract-leaving-confirmation-popup.component';
import { IContractDetailed } from './models/contract-detailed.interface';
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

  public get canEditContract(): boolean {
    return this.rightsService.hasOperation(Operation.EditContracts);
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  public get hasFormChanged(): boolean {
    return !this.isEqual(this.contractToEdit$.value, this.contractForm.value);
  }

  private contractToEdit$: BehaviorSubject<IContractDetailed> = new BehaviorSubject<IContractDetailed>(null);
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private rightsService: RightsService,
    private activatedRoute: ActivatedRoute,
    private contractTabService: ContractTabService,
    private contractsManageModalService: ContractManagementService,
    private contractsListService: ContractsListService,
    private contextStoreService: ValidationContextStoreService,
    private distributorsService: DistributorsService,
    private luPopup: LuPopup,
  ) {}

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.contextStoreService.context$
      .pipe(take(1))
      .subscribe(contract => this.contractForm.setValue(this.toContractForm(contract)));

    this.contractToEdit$
      .pipe(take(1), switchMap(contract => this.toFormInformation$(contract)))
      .subscribe(this.formInformation$);

    this.contractTabService.getContractDetailed$(this.contractId)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(contract => {
        this.contractForm.setValue(this.toContractForm(contract));
        this.contractToEdit$.next(contract);
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
      .subscribe(buttonState => this.editButtonState$.next(buttonState));
  }

  public close(): void {
    if (!this.hasFormChanged) {
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

  private isEqual(contract: IContractDetailed, form: IContractForm): boolean {
    return contract.distributor.id === form.distributor.id
      && contract.client.id === form.client.id
      && contract.offer.id === form.offer.id
      && contract.product.id === form.product.id
      && contract.billingMonth === form.billingMonth
      && isDateEqual(new Date(contract.theoricalStartOn), new Date(form.theoreticalStartOn))
      && contract.clientRebate === form.clientRebate.count
      && isDateEqual(new Date(contract.endClientRebateOn), new Date(form.clientRebate.endAt))
      && contract.nbMonthTheorical === form.theoreticalMonthRebate
      && contract.minimalBillingPercentage === form.minimalBillingPercentage
      && contract.unityNumberTheorical === form.theoreticalDraftCount
      && contract.comment === form.comment;
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
