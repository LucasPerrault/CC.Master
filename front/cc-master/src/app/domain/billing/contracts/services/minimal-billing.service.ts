import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3Response } from '@cc/common/queries';
import { sfLuccaDistributorId } from '@cc/domain/billing/distributors';
import {
  IProductMinimalBillingEligibility,
  productMinimalBillingEligibilityFields,
} from '@cc/domain/billing/offers/models/product-minimal-billing-eligibility.interface';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { IContractMinimalBillable } from '../models/contract-minimal-billable.interface';

@Injectable()
export class MinimalBillingService {

  public readonly minimalBillingPercentage = 75;
  private readonly productsEndPoint = '/api/v3/products';

  constructor(private httpClient: HttpClient) {}

  public isEligibleForMinimalBilling$(contract: IContractMinimalBillable): Observable<boolean> {
    if (!this.isDirectSales(contract) || this.hasTheoreticalMonthRebate(contract)) {
      return of(false);
    }

    return this.isProductEligible$(contract);
  };

  private hasTheoreticalMonthRebate(contract: IContractMinimalBillable): boolean {
    return contract.theoreticalMonthRebate > 0;
  }

  private isDirectSales(contract: IContractMinimalBillable): boolean {
    return !!contract.distributorId && contract.distributorId === sfLuccaDistributorId;
  }

  private isProductEligible$(contract: IContractMinimalBillable): Observable<boolean> {
    if (!contract.productId) {
      return of(false);
    }

    const url = `${this.productsEndPoint}/${contract.productId}`;
    const params = new HttpParams().set('fields', productMinimalBillingEligibilityFields);

    return this.httpClient.get<IHttpApiV3Response<IProductMinimalBillingEligibility>>(url, { params })
      .pipe(
        map(response => response.data),
        map(data => data.isEligibleToMinimalBilling),
      );
  }
}
