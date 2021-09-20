import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { DistributorFilter } from '../../common/distributor-filter-button-group';
import { IContractsDraftFilter } from '../models';

enum ContractDraftQueryParamKey {
  IsDirectSales = 'isDirectSales',
  Name = 'name',
}

@Injectable()
export class ContractsDraftApiMappingService {

  public toHttpParams(filters?: IContractsDraftFilter): HttpParams {
    let params = new HttpParams();
    if (!filters) {
      return params;
    }

    params = this.setIsDirectSales(params, filters.saleType);
    return this.setContractDraftName(params, filters.draftName);
  }

  private setIsDirectSales(params: HttpParams, saleType: DistributorFilter): HttpParams {
    if (saleType === DistributorFilter.All) {
      return params.delete(ContractDraftQueryParamKey.IsDirectSales);
    }

    const isDirectSales = saleType === DistributorFilter.Direct;
    return params.set(ContractDraftQueryParamKey.IsDirectSales, isDirectSales.toString());
  }

  private setContractDraftName(params: HttpParams, name: string): HttpParams {
    if (!name) {
      return params.delete(ContractDraftQueryParamKey.Name);
    }

    return params.set(ContractDraftQueryParamKey.Name, `like,${name}`);
  }
}
