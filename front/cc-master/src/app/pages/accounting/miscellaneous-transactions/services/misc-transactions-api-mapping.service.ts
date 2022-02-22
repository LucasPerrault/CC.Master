import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IBillingEntity } from '@cc/domain/billing/clients';
import { IContract } from '@cc/domain/billing/contracts/v4';

import { IMiscTransactionsFilter } from '../models/misc-transactions-filter.interface';

enum MiscTransactionsQueryParamKey {
  Contracts = 'contractId',
  BillingEntities = 'contract.client.billingEntity',
}

@Injectable()
export class MiscTransactionsApiMappingService {
  public toHttpParams(filters: IMiscTransactionsFilter): HttpParams {
    let params = new HttpParams();
    if (!filters) {
      return params;
    }

    params = this.setContracts(params, filters.contracts);
    return this.setBillingEntities(params, filters.billingEntities);
  }

  private setContracts(params: HttpParams, contracts: IContract[]) {
    if (!contracts?.length) {
      return params.delete(MiscTransactionsQueryParamKey.Contracts);
    }

    const contractIds = contracts?.map(b => b.id) ?? [];
    return params.set(MiscTransactionsQueryParamKey.Contracts, contractIds.join(','));  }

  private setBillingEntities(params: HttpParams, billingEntities: IBillingEntity[]) {
    if (!billingEntities?.length) {
      return params.delete(MiscTransactionsQueryParamKey.BillingEntities);
    }

    const billingEntityIds = billingEntities?.map(b => b.id) ?? [];
    return params.set(MiscTransactionsQueryParamKey.BillingEntities, billingEntityIds.join(','));
  }
}
