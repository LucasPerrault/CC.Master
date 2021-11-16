import { ChangeDetectionStrategy, Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { AbstractControl, FormArray, FormControl, FormGroup } from '@angular/forms';

import { getBillingMode } from '../../../enums/billing-mode.enum';
import { getBillingUnit } from '../../../enums/billing-unit.enum';
import { getCurrency } from '../../../models/offer-currency.interface';
import { IUploadedOffer } from '../uploaded-offer.interface';

enum ImportedOfferFormKey {
  Name = 'name',
  Product ='product',
  BillingUnit = 'billingUnit',
  SageBusiness = 'sageBusiness',
  Tag = 'tag',
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
  changeDetection: ChangeDetectionStrategy.OnPush,
  encapsulation: ViewEncapsulation.None,
})
export class OfferImportTableComponent implements OnInit {
  @Input() public set offers(importedOffers: IUploadedOffer[]) {
    this.reset(importedOffers);
  }

  public formArrayKey = 'importedOffers';
  public formArray: FormArray = new FormArray([]);
  public formKey = ImportedOfferFormKey;
  public formGroup: FormGroup = new FormGroup(
    { [this.formArrayKey]: this.formArray },
  );

  constructor() { }

  public ngOnInit(): void {

  }

  public trackBy(index: number, offer: AbstractControl): AbstractControl {
    return offer;
  }

  private reset(offers: IUploadedOffer[]): void {
    this.formArray.clear();
    this.addRange(offers);
  }

  private addRange(offers: IUploadedOffer[]): void {
    offers.forEach(offer => this.add(offer));
  }

  private add(offer: IUploadedOffer): void {
    const formGroup = this.getFormGroup(offer);
    this.formArray.push(formGroup);
  }

  private getFormGroup(offer: IUploadedOffer): FormGroup {
    return new FormGroup(
      {
        [ImportedOfferFormKey.Name]: new FormControl(offer?.name),
        [ImportedOfferFormKey.Product]: new FormControl(offer?.product),
        [ImportedOfferFormKey.BillingUnit]: new FormControl(getBillingUnit(offer?.billingUnit)),
        [ImportedOfferFormKey.SageBusiness]: new FormControl(offer?.sageBusiness),
        [ImportedOfferFormKey.Tag]: new FormControl(offer?.category),
        [ImportedOfferFormKey.BillingMode]: new FormControl(getBillingMode(offer?.billingMode)),
        [ImportedOfferFormKey.PricingMethod]: new FormControl(offer?.pricingMethod),
        [ImportedOfferFormKey.ForecastMethod]: new FormControl(offer?.forecastMethod),
        [ImportedOfferFormKey.Currency]: new FormControl(getCurrency(offer?.currencyID)),
        [ImportedOfferFormKey.PriceLists]: new FormControl(offer?.priceLists),
      },
    );
  }
}
