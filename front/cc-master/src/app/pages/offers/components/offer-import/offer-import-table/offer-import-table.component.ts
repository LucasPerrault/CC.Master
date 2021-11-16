import { Component, Input } from '@angular/core';

import { BillingMode, getBillingMode } from '../../../enums/billing-mode.enum';
import { BillingUnit, getBillingUnit } from '../../../enums/billing-unit.enum';
import { CurrencyCode, getCurrency } from '../../../models/offer-currency.interface';
import { IImportedOffer } from '../imported-offer.interface';

@Component({
  selector: 'cc-offer-import-table',
  templateUrl: './offer-import-table.component.html',
  styleUrls: ['./offer-import-table.component.scss'],
})
export class OfferImportTableComponent {
  @Input() public offers: IImportedOffer[];

  constructor() { }

  public trackBy(index: number, offer: IImportedOffer): string {
    return offer.name;
  }

  public getBillingUnit(unit: BillingUnit): string {
    return getBillingUnit(unit)?.name;
  }

  public getBillingMode(billingMode: BillingMode): string {
    return getBillingMode(billingMode)?.name;
  }

  public getCurrencyName(code: CurrencyCode): string {
    return getCurrency(code)?.name;
  }
}
