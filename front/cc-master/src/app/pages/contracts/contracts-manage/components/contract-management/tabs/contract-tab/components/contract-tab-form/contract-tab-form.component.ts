import { ChangeDetectionStrategy, Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
} from '@angular/forms';
import { SelectDisplayMode } from '@cc/common/forms';
import { ContractBillingMonth, IContractForm } from '@cc/domain/billing/contracts';
import { LuModal } from '@lucca-front/ng/modal';
import { ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IValidationContext } from '../../../../validation-context-store.data';
import { IContractFormInformation } from '../../models/contract-form-information.interface';
import { ContractActionRestrictionsService } from '../../services/contract-action-restrictions.service.';
import { ClientInfoModalComponent } from './client-info-modal/client-info-modal.component';
import { IClientInfoModalData } from './client-info-modal/client-info-modal-data.interface';

enum ContractFormKey {
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
  selector: 'cc-contract-tab-form',
  templateUrl: './contract-tab-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: ContractTabFormComponent,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ContractTabFormComponent,
    },
  ],
})
export class ContractTabFormComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public validationContext: IValidationContext;
  @Input() public formInformation: IContractFormInformation;

  public onChange: (contractForm: IContractForm) => void;
  public onTouch: () => void;

  public formGroup: FormGroup;
  public formGroupKey = ContractFormKey;
  public selectDisplayMode = SelectDisplayMode;

  public isFrequencyDisabled$: ReplaySubject<boolean> = new ReplaySubject<boolean>();

  public get offerApiFilters(): string[] {
    const product = this.formGroup.get(ContractFormKey.Product).value;
    if (!product) {
      return [];
    }

    return [`productId=${product.id}`];
  }

  public get canEditTheoreticalStartOn(): boolean {
    return this.formValidationService.canEditTheoreticalStartOn(this.validationContext);
  }

  public get canEditDistributor(): boolean {
    return this.formValidationService.canEditDistributor(this.validationContext);
  }

  public get canEditClient(): boolean {
    return this.formValidationService.canEditClient(this.validationContext);
  }

  public get canEditOffer(): boolean {
    return this.formValidationService.canEditOffer(this.validationContext);
  }

  public get canEditProduct(): boolean {
    return this.formValidationService.canEditProduct(this.validationContext);
  }

  public get canEditMinimalBilling(): boolean {
    return this.formValidationService.canEditMinimalBilling(this.validationContext);
  }

  public get canEditBillingFrequency(): boolean {
    return this.formValidationService.canEditBillingFrequency(this.validationContext);
  }

  private get canEditContract(): boolean {
    return this.formValidationService.canEditContract();
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private formValidationService: ContractActionRestrictionsService, private luModal: LuModal) {
  }

  public ngOnInit(): void {
    this.formGroup = new FormGroup({
      [ContractFormKey.BillingMonth]: new FormControl({ value: null }),
      [ContractFormKey.Distributor]: new FormControl({ value: null, disabled: !this.canEditDistributor }),
      [ContractFormKey.Client]: new FormControl({ value: null, disabled: !this.canEditClient }),
      [ContractFormKey.Offer]: new FormControl({ value: null, disabled: !this.canEditOffer }),
      [ContractFormKey.Product]: new FormControl({ value: null, disabled: !this.canEditProduct }),
      [ContractFormKey.TheoreticalDraftCount]: new FormControl({ value: 0, disabled: !this.canEditContract }),
      [ContractFormKey.ClientRebate]: new FormControl({
        value: { count: null, endAt: null },
        disabled: !this.canEditContract,
      }),
      [ContractFormKey.TheoreticalMonthRebate]: new FormControl({ value: 0 }),
      [ContractFormKey.TheoreticalStartOn]: new FormControl({ value: null, disabled: !this.canEditTheoreticalStartOn }),
      [ContractFormKey.MinimalBillingPercentage]: new FormControl({ value: 0, disabled: !this.canEditMinimalBilling }),
      [ContractFormKey.Comment]: new FormControl({ value: '', disabled: !this.canEditContract }),
    });

    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.onChange(this.formGroup.getRawValue()));

    this.formGroup.get(ContractFormKey.Product).valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.formGroup.get(ContractFormKey.Offer).reset(null));

    this.formGroup.get(ContractFormKey.TheoreticalStartOn).valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(startOn => this.updateBillingFrequency(startOn));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(contractForm: IContractForm): void {
    if (!!contractForm && this.formGroup.value !== contractForm) {
      this.formGroup.setValue(contractForm, { emitEvent: false });
      this.setBillingMonthDisabled(contractForm.billingMonth);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (!this.formGroup || this.formGroup.invalid) {
      return { invalid: true };
    }
  }

  public openClientInformationModal(): void {
    const data: IClientInfoModalData = {
      salesforceId: this.formInformation.client?.salesforceId,
      commercialManagementId: this.formInformation.client?.commercialManagementId,
      name: this.formInformation.client?.name,
    };
    this.luModal.open(ClientInfoModalComponent, data);
  }

  public hasRequiredError(formKey: ContractFormKey): boolean {
    const ctrl = this.formGroup.get(formKey);
    return ctrl.touched && ctrl.hasError('required');
  }

  private setBillingMonthDisabled(billingMonth: ContractBillingMonth): void {
    const billingFrequencyAndMonth = this.formGroup.get(ContractFormKey.BillingMonth);
    if (this.canEditBillingFrequency) {
      billingFrequencyAndMonth.enable({ emitEvent: false });
      return;
    }

    if (!this.canEditBillingFrequency && billingMonth !== ContractBillingMonth.Quarterly) {
      this.isFrequencyDisabled$.next(true);
      return;
    }

    billingFrequencyAndMonth.disable({ emitEvent: false });
  }

  private updateBillingFrequency(startOn: string) {
    const billingFrequency = this.formGroup.get(ContractFormKey.BillingMonth).value;
    if (billingFrequency === ContractBillingMonth.Quarterly) {
      return;
    }

    const startDate = new Date(startOn);
    const billingMonth = startDate.getMonth() + 1;
    this.formGroup.get(ContractFormKey.BillingMonth).setValue(billingMonth);
  }
}
