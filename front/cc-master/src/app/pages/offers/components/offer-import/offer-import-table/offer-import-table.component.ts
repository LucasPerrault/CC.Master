import { Component, forwardRef, Input, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormControl,
  FormGroup, NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { LuModal } from '@lucca-front/ng/modal';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { getBillingMode } from '../../../enums/billing-mode.enum';
import { getBillingUnit } from '../../../enums/billing-unit.enum';
import { getCurrency } from '../../../models/offer-currency.interface';
import { IPriceListForm } from '../../../models/price-list-form.interface';
import { PriceListsValidators } from '../../../services/price-lists.validators';
import { ImportedPriceListsModalComponent } from '../imported-price-lists-modal/imported-price-lists-modal.component';
import { IImportedPriceListsModalData } from '../imported-price-lists-modal/imported-price-lists-modal-data.interface';
import { IUploadedOffer } from '../uploaded-offer-dto.interface';
import { IUploadedOfferForm } from '../uploaded-offer-form.interface';

enum ImportedOfferFormKey {
  Name = 'name',
  Product ='product',
  BillingUnit = 'billingUnit',
  Tag = 'category',
  BillingMode = 'billingMode',
  Currency = 'currency',
  ForecastMethod = 'forecastMethod',
  PricingMethod = 'pricingMethod',
  PriceLists = 'priceLists',
}

@Component({
  selector: 'cc-offer-import-table',
  templateUrl: './offer-import-table.component.html',
  styleUrls: ['./offer-import-table.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => OfferImportTableComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: OfferImportTableComponent,
    },
  ],
})
export class OfferImportTableComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public readonly = true;

  public formArrayKey = 'importedOffers';
  public formArray: FormArray = new FormArray([]);
  public formKey = ImportedOfferFormKey;
  public formGroup: FormGroup = new FormGroup(
    { [this.formArrayKey]: this.formArray },
  );

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private luModal: LuModal) { }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.onChange(this.toUploadOffers(this.formArray.value)));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (uploadedOffers: IUploadedOffer[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(uploadedOffers: IUploadedOffer[]): void {
    this.reset(uploadedOffers);
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }

  public trackBy(index: number, offer: AbstractControl): AbstractControl {
    return offer;
  }

  public getPriceLists(control: AbstractControl): IPriceListForm[] {
    return control.get(ImportedOfferFormKey.PriceLists).value ?? [];
  }

  public openPriceListsDetails(priceLists: IPriceListForm[], control: AbstractControl) {
    const data: IImportedPriceListsModalData = { priceLists, readonly: this.readonly };
    const dialogRef = this.luModal.open(ImportedPriceListsModalComponent, data, {
      panelClass: ['lu-popup-panel', 'mod-widthAuto'],
    });

    dialogRef.onClose
      .pipe(takeUntil(this.destroy$))
      .subscribe(lists => control.get(ImportedOfferFormKey.PriceLists).reset(lists));
  }

  private reset(offers: IUploadedOffer[]): void {
    this.formArray.clear();
    this.addRange(offers);
  }

  private addRange(offers: IUploadedOffer[]): void {
    offers?.forEach(offer => this.add(offer));
  }

  private add(offer: IUploadedOffer): void {
    const formGroup = this.getFormGroup(offer);
    this.formArray.push(formGroup);
  }

  private getFormGroup(offer: IUploadedOffer): FormGroup {
    return new FormGroup(
      {
        [ImportedOfferFormKey.Name]: new FormControl(offer?.name, [Validators.required]),
        [ImportedOfferFormKey.Product]: new FormControl(offer?.product, [Validators.required]),
        [ImportedOfferFormKey.BillingUnit]: new FormControl(getBillingUnit(offer?.billingUnit), [Validators.required]),
        [ImportedOfferFormKey.Tag]: new FormControl(offer?.category, [Validators.required]),
        [ImportedOfferFormKey.BillingMode]: new FormControl(getBillingMode(offer?.billingMode), [Validators.required]),
        [ImportedOfferFormKey.PricingMethod]: new FormControl(offer?.pricingMethod, [Validators.required]),
        [ImportedOfferFormKey.ForecastMethod]: new FormControl(offer?.forecastMethod, [Validators.required]),
        [ImportedOfferFormKey.Currency]: new FormControl(getCurrency(offer?.currencyId), [Validators.required]),
        [ImportedOfferFormKey.PriceLists]: new FormControl(offer?.priceLists, [
          PriceListsValidators.uniqStartsOnRange,
          PriceListsValidators.boundsContinuityRange,
          PriceListsValidators.startsOnFirstDayOfTheMonthRange,
          PriceListsValidators.validPricesRange,
          Validators.required,
        ]),
      },
    );
  }

  private toUploadOffers(offers: IUploadedOfferForm[]): IUploadedOffer[] {
    return offers.map(offer => ({
      name: offer.name,
      priceLists: offer.priceLists,
      forecastMethod: offer.forecastMethod,
      pricingMethod: offer.pricingMethod,
      category: offer.category,
      product: offer.product,
      currencyId: offer.currency?.code,
      billingMode: offer.billingMode?.id,
      billingUnit: offer.billingUnit?.id,
    }));
  }
}
