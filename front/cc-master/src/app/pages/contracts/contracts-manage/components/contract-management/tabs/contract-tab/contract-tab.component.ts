import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Operation, RightsService } from '@cc/aspects/rights';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { IContractForm } from '@cc/domain/billing/contracts';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, startWith, take } from 'rxjs/operators';

import { ContractsListService } from '../../../../services/contracts-list.service';
import { ContractManagementService } from '../../contract-management.service';
import { IContractDetailed } from './models/contract-detailed.interface';
import { IContractValidationContext } from './models/contract-validation-context.interface';
import { ContractActionRestrictionsService } from './services/contract-action-restrictions.service.';
import { ContractTabService } from './services/contract-tab.service';
import { ContractValidationContextService } from './services/contract-validation-context.service';

@Component({
  selector: 'cc-contract-tab',
  templateUrl: './contract-tab.component.html',
  styleUrls: ['./contract-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContractTabComponent implements OnInit {

  public contractForm: FormControl = new FormControl();
  public validationContext$: ReplaySubject<IContractValidationContext> = new ReplaySubject();
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

  constructor(
    private rightsService: RightsService,
    private activatedRoute: ActivatedRoute,
    private contractTabService: ContractTabService,
    private contractFormValidationService: ContractActionRestrictionsService,
    private contractValidationContextService: ContractValidationContextService,
    private contractsManageModalService: ContractManagementService,
    private contractsListService: ContractsListService,
  ) {}

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.contractValidationContextService.getValidationContext$(this.contractId)
      .pipe(take(1))
      .subscribe(context => this.validationContext$.next(context));

    this.contractTabService.getContractDetailed$(this.contractId)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(contract => this.contractForm.setValue(this.toContractForm(contract)));
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
}
