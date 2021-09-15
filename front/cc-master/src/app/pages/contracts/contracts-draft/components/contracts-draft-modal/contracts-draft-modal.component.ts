import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { ContractsService, IContract, IContractForm } from '@cc/domain/billing/contracts';
import { ILuSidepanelContent, LU_SIDEPANEL_DATA, LuSidepanel } from '@lucca-front/ng/sidepanel';
import { BehaviorSubject, Observable, Subject, throwError } from 'rxjs';
import { catchError, finalize, takeUntil } from 'rxjs/operators';

import { IContractDraftForm, IContractDraftFormInformation } from '../../models';
import { ContractsDraftService } from '../../services';
import { ContractsDraftListService } from '../../services/contracts-draft-list.service';

@Component({
  selector: 'cc-contracts-draft-entry-modal',
  template: '',
})
export class ContractsDraftEntryModalComponent {

  constructor(
    private luSidepanel: LuSidepanel,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private draftListService: ContractsDraftListService,
  ) {
    const draftId = this.getContractDraftId(this.activatedRoute.snapshot.paramMap);
    const dialog = this.luSidepanel.open(ContractsDraftModalComponent, draftId);

    dialog.onClose.subscribe(async () => {
      this.draftListService.update();
      await this.redirectToParentAsync();
    });
    dialog.onDismiss.subscribe(async () => await this.redirectToParentAsync());
  }

  private getContractDraftId(routingParams: ParamMap): string {
    if (!routingParams.has('id')) {
      return null;
    }

    return routingParams.get('id');
  }

  private async redirectToParentAsync(): Promise<void> {
    await this.router.navigate(['.'], {
      relativeTo: this.activatedRoute.parent,
    });
  }
}


@Component({
  selector: 'cc-contracts-draft-modal',
  templateUrl: './contracts-draft-modal.component.html',
})
export class ContractsDraftModalComponent implements ILuSidepanelContent, OnInit, OnDestroy {
  public draftFormInformation$: BehaviorSubject<IContractDraftFormInformation>
    = new BehaviorSubject<IContractDraftFormInformation>(null);
  public isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  public title = this.translatePipe.transform('front_draftPage_modal_title');
  public submitLabel = this.translatePipe.transform('front_draftPage_modal_submitLabel');
  public submitDisabled = true;

  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();
  private isProvidedFromOpportunity = false;

  constructor(
    private translatePipe: TranslatePipe,
    private contractsService: ContractsService,
    private contractDraftService: ContractsDraftService,
    @Inject(LU_SIDEPANEL_DATA) private draftId?: string,
  ) {
  }

  public ngOnInit(): void {
    this.formControl.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formControl.invalid);

    if (!!this.draftId) {
      this.isLoading$.next(true);
      this.contractDraftService.getContractDraft$(this.draftId)
        .pipe(finalize(() => this.isLoading$.next(false)))
        .subscribe(draft => {
          this.formControl.setValue(this.getContractForm(draft));
          this.draftFormInformation$.next(this.getDraftFormInformation(draft));
          this.isProvidedFromOpportunity = true;
        });
    }
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<IContract> {
    return this.isProvidedFromOpportunity ? this.convert$(this.draftId) : this.create$();
  }

  private convert$(draftId: string): Observable<IContract> {
    return this.contractDraftService.convertToContract$(draftId, this.formControl.value).pipe(
      catchError((err: HttpErrorResponse) =>
        throwError(`${ err.status }: ${ err.statusText }`),
      ),
    );
  }

  private create$(): Observable<IContract> {
    return this.contractsService.createContract$(this.formControl.value).pipe(
      catchError((err: HttpErrorResponse) =>
        throwError(`${ err.status }: ${ err.statusText }`),
      ),
    );
  }

  private getDraftFormInformation(c: IContractDraftForm): IContractDraftFormInformation {
    return ({
      externalDeploymentAt: c.opportunityLineItemDetail.deploymentOn,
      externalOfferName: c.opportunityLineItemDetail.productName,
      externalDistributorUrl: c.externalDistributorUrl,
    });
  }

  private getContractForm(c: IContractDraftForm): IContractForm {
    return ({
      billingMonth: c.billingMonth,
      distributor: c.distributor,
      client: c.client,
      offer: c.offer,
      product: c.product,
      theoreticalDraftCount: c.unityNumberTheorical,
      clientRebate: {
        count: c.clientRebate,
        endAt: !!c.endClientRebateOn ? new Date(c.endClientRebateOn) : null,
      },
      theoreticalMonthRebate: c.nbMonthTheorical,
      theoreticalStartOn: new Date(c.theoricalStartOn),
      minimalBillingPercentage: c.minimalBillingPercentage,
      comment: c.comment,
    });
  }
}
