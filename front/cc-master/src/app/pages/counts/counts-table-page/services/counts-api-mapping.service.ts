import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IDateRange } from '@cc/common/date';
import { ApiSortHelper, ApiV3DateService } from '@cc/common/queries';
import { ISortParams } from '@cc/common/sort';
import { IClient } from '@cc/domain/billing/clients';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, IProduct } from '@cc/domain/billing/offers';
import { IEnvironment, IEnvironmentGroup } from '@cc/domain/environments';

import { ICountsFilterForm } from '../models/counts-filter-form.interface';

interface HttpParamsAttributes {
  filters: ICountsFilterForm;
  sort?: ISortParams;
}

enum CountsQueryParamKey {
  CountPeriod = 'countPeriod',
  Client = 'contract.clientId',
  Distributor = 'contract.distributorId',
  Offer = 'offerId',
  EnvironmentGroup = 'contract.environment.groupId',
  Product = 'offer.productId',
  EnvironmentId = 'contract.environment.id'
}

@Injectable()
export class CountsApiMappingService {
  constructor(private apiV3DateService: ApiV3DateService) {}

  public toHttpParams(attributes: HttpParamsAttributes): HttpParams {
    let params = new HttpParams();
    params = this.setSortParams(params, attributes.sort);
    return this.setFilters(params, attributes.filters);
  }

  private setSortParams(params: HttpParams, sortParams: ISortParams): HttpParams {
    if (!sortParams) {
      return params;
    }

    return ApiSortHelper.toV3HttpParams(params, sortParams);
  }

  private setFilters(params: HttpParams, filters: ICountsFilterForm): HttpParams {
    if (!filters) {
      return params;
    }

    params = this.getCountPeriodHttpParams(params, filters.countPeriod);
    params = this.getClientsHttpParams(params, filters.clients);
    params = this.getDistributorsHttpParams(params, filters.distributors);
    params = this.getOffersHttpParams(params, filters.offers);
    params = this.getEnvironmentGroupsHttpParams(params, filters.environmentGroups);
    params = this.getProductsHttpParams(params, filters.products);
    params = this.getEnvironmentsHttpParams(params, filters.environments);
    return params;
  }

  private getCountPeriodHttpParams(params: HttpParams, dateRange: IDateRange): HttpParams {
    if (!dateRange?.startDate && !dateRange?.endDate) {
      return params.delete(CountsQueryParamKey.CountPeriod);
    }

    const apiV3CreatedOn = this.apiV3DateService.toApiDateRangeFormat(dateRange);
    return params.set(CountsQueryParamKey.CountPeriod, apiV3CreatedOn);
  }

  private getClientsHttpParams(params: HttpParams, clients: IClient[]): HttpParams {
    return !!clients?.length
      ? params.set(CountsQueryParamKey.Client, clients.map(c => c.id).join(','))
      : params.delete(CountsQueryParamKey.Client);
  }

  private getDistributorsHttpParams(params: HttpParams, distributors: IDistributor[]): HttpParams {
    return !!distributors?.length
      ? params.set(CountsQueryParamKey.Distributor, distributors.map(c => c.id).join(','))
      : params.delete(CountsQueryParamKey.Distributor);
  }

  private getOffersHttpParams(params: HttpParams, offers: IOffer[]): HttpParams {
    return !!offers?.length
      ? params.set(CountsQueryParamKey.Offer, offers.map(c => c.id).join(','))
      : params.delete(CountsQueryParamKey.Offer);
  }

  private getEnvironmentGroupsHttpParams(params: HttpParams, environmentGroups: IEnvironmentGroup[]): HttpParams {
    return !!environmentGroups?.length
      ? params.set(CountsQueryParamKey.EnvironmentGroup, environmentGroups.map(c => c.id).join(','))
      : params.delete(CountsQueryParamKey.EnvironmentGroup);
  }

  private getProductsHttpParams(params: HttpParams, products: IProduct[]): HttpParams {
    return !!products?.length
      ? params.set(CountsQueryParamKey.Product, products.map(c => c.id).join(','))
      : params.delete(CountsQueryParamKey.Product);
  }

  private getEnvironmentsHttpParams(params: HttpParams, environments: IEnvironment[]) {
    return !!environments?.length
      ? params.set(CountsQueryParamKey.EnvironmentId, environments.map(c => c.id).join(','))
      : params.delete(CountsQueryParamKey.EnvironmentId);
  }
}
