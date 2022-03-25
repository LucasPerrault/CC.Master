import { ChangeDetectionStrategy, Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { SelectDisplayMode } from '@cc/common/forms';
import { BILLING_CORE_DATA, getNameById, IBillingCoreData } from '@cc/domain/billing/billling-core-data';
import { IContractForm, MinimalBillingService } from '@cc/domain/billing/contracts';
import { DistributorsService, IDistributor } from '@cc/domain/billing/distributors';
import { IProduct } from '@cc/domain/billing/offers';
import { LuModal } from '@lucca-front/ng/modal';
import { BehaviorSubject, combineLatest, merge, Observable, pipe, Subject, UnaryFunction } from 'rxjs';
import { filter, map, switchMap, takeUntil } from 'rxjs/operators';

import { PriceGridModalComponent } from '../../../common/price-grid-modal/price-grid-modal.component';
import { IContractDraftFormInformation } from '../../models';

enum DraftFormKey {
  BillingMonth = 'billingMonth',
  Distributor = 'distributor',
  Client = 'client',
  Offer = 'offer',
  Product = 'product',
  TheoreticalDraftCount = 'theoreticalDraftCount',
  ClientRebate = 'clientRebate',
  TheoreticalMonthRebate = 'theoreticalMonthRebate',
  TheoreticalStartOn = 'theoreticalStartOn',
  MinimalBillingPercentage ='minimalBillingPercentage',
  Comment = 'comment',
}

@Component({
  selector: 'cc-contracts-draft-form',
  templateUrl: './contracts-draft-form.component.html',
  styleUrls: ['./contracts-draft-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: ContractsDraftFormComponent,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ContractsDraftFormComponent,
    },
  ],
})
export class ContractsDraftFormComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public draftFormInformation: IContractDraftFormInformation;

  public formGroup: FormGroup;
  public formGroupKey = DraftFormKey;

  public selectDisplayMode = SelectDisplayMode;
  public distributorRebate$: BehaviorSubject<number> = new BehaviorSubject<number>(0);

  public get distributor(): IDistributor {
    return this.formGroup.get(DraftFormKey.Distributor).value;
  }

  public get offerApiFilters(): string[] {
    const filters = ['isArchived=false'];
    const product = this.formGroup.get(DraftFormKey.Product).value;
    if (!!product) {
      filters.push(`productId=${product.id}`);
    }

    return filters;
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private distributorsService: DistributorsService,
    private minimalBillingService: MinimalBillingService,
    private translatePipe: TranslatePipe,
    private luModal: LuModal,
    @Inject(BILLING_CORE_DATA) private billingCoreData: IBillingCoreData,
  ) {
    this.formGroup = new FormGroup({
      [DraftFormKey.BillingMonth]: new FormControl(null),
      [DraftFormKey.Distributor]: new FormControl(null),
      [DraftFormKey.Client]: new FormControl(null),
      [DraftFormKey.Offer]: new FormControl(null),
      [DraftFormKey.Product]: new FormControl(null),
      [DraftFormKey.TheoreticalDraftCount]: new FormControl(0),
      [DraftFormKey.ClientRebate]: new FormControl({ count: null, endAt: null }),
      [DraftFormKey.TheoreticalMonthRebate]: new FormControl(0),
      [DraftFormKey.TheoreticalStartOn]: new FormControl(null),
      [DraftFormKey.MinimalBillingPercentage]: new FormControl(0),
      [DraftFormKey.Comment]: new FormControl(''),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(f => this.onChange(f));

    this.formGroup.get(DraftFormKey.Product).valueChanges
      .pipe(takeUntil(this.destroy$), filter(() => this.formGroup.get(DraftFormKey.Product).dirty))
      .subscribe(() => this.formGroup.get(DraftFormKey.Offer).reset(null));

    merge(
      this.formGroup.get(DraftFormKey.Product).valueChanges,
      this.formGroup.get(DraftFormKey.Distributor).valueChanges,
    )
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.updateDistributorRebate());

    combineLatest([
      this.formGroup.get(DraftFormKey.Product).valueChanges,
      this.formGroup.get(DraftFormKey.Distributor).valueChanges,
      this.formGroup.get(DraftFormKey.TheoreticalMonthRebate).valueChanges,
    ])
      .pipe(takeUntil(this.destroy$), this.toMinimalBillingEligibility)
      .subscribe(isEligible => this.updateMinimalBilling(isEligible));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onTouch: () => void = () => {};
  public onChange: (contractForm: IContractForm) => void = () => {};

  registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  writeValue(contractForm: IContractForm): void {
    if (!!contractForm && contractForm !== this.formGroup.value) {
      this.formGroup.setValue(contractForm);
    }
  }

  public openPriceGridModal(): void {
    const offerId = this.formGroup.get(DraftFormKey.Offer).value?.id;
    const contractStartOn = this.formGroup.get(DraftFormKey.TheoreticalStartOn).value;
    this.luModal.open(PriceGridModalComponent, { offerId, contractStartOn });
  }

  public redirectToExternalDistributorUrl(): void {
    if (!this.distributor) {
      return;
    }

    const salesforceUrl = 'https://eu4.salesforce.com';
    window.open(`${ salesforceUrl }/${ this.distributor.salesforceId }`);
  }

  public getBillingEntityName(id: number): string {
    return getNameById(this.billingCoreData.billingEntities, id);
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (!this.formGroup || this.formGroup.invalid) {
      return { invalid: true };
    }
  }

  public hasRequiredError(formKey: DraftFormKey): boolean {
    const ctrl = this.formGroup.get(formKey);
    return ctrl.touched && ctrl.hasError('required');
  }

  private updateMinimalBilling(isEligible: boolean): void {
    const minimalBillingPercentage = this.formGroup.get(DraftFormKey.MinimalBillingPercentage);

    if (!isEligible) {
      minimalBillingPercentage.setValue(0);
      minimalBillingPercentage.disable();
      return;
    }

    minimalBillingPercentage.enable();
    if (!this.isMoreThanZero(minimalBillingPercentage.value)) {
      minimalBillingPercentage.setValue(this.minimalBillingService.defaultPercentage);
    }
  }

  private updateDistributorRebate(): void {
    const productId = this.formGroup.get(DraftFormKey.Product).value?.id;
    const distributorId = this.formGroup.get(DraftFormKey.Distributor).value?.id;
    if (!distributorId || !productId) {
      this.distributorRebate$.next(null);
      return;
    }

    const activeRebateForProduct$ = this.distributorsService.getActiveRebate$(distributorId, productId);
    activeRebateForProduct$.subscribe(r => this.distributorRebate$.next(r));
  }

  private isMoreThanZero(minimalBillingPercentage: number): boolean {
    return !!minimalBillingPercentage;
  }

  private get toMinimalBillingEligibility(): UnaryFunction<Observable<[IProduct, IDistributor, number]>, Observable<boolean>> {
    return pipe(
      map(([product, distributor, theoreticalMonthRebate]) => ({ theoreticalMonthRebate, distributor, productId: product?.id })),
      switchMap(minimalBillable => this.minimalBillingService.isEligibleForMinimalBilling$(minimalBillable)),
    );
  }
}
