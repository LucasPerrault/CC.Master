import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService, apiV3SortToHttpParams, toApiV3SortParams } from '@cc/common/queries';
import { ISortParams } from '@cc/common/sort';
import { IClient } from '@cc/domain/billing/clients';
import { IDistributor } from '@cc/domain/billing/distributors';
import { IEstablishment } from '@cc/domain/billing/establishments';
import { IOffer, IProduct } from '@cc/domain/billing/offers';
import { IEnvironment } from '@cc/domain/environments';

import { DistributorFilter } from '../../common/distributor-filter-button-group';
import { ContractEstablishmentHealth } from '../constants/contract-establishment-health.enum';
import { IContractEstablishmentHealth } from '../models/contract-establishment-health.interface';
import { IContractState } from '../models/contract-state.interface';
import { IContractsFilter } from '../models/contracts-filter.interface';

export class HttpParamsAttributes {
  sort: ISortParams;
  filters: IContractsFilter;
}

enum ContractQueryParamKey {
  Id = 'id',
  Name = 'name',
  IsDirectSales = 'isDirectSales',
  ClientId = 'client.id',
  DistributorId = 'distributor.id',
  ProductId = 'product.id',
  OfferId = 'offer.id',
  EnvironmentId = 'environmentId',
  EstablishmentId = 'activeLegalEntities.id',
  State = 'state',
  CreatedOn = 'createdOn',
  TheoreticalStartOn = 'theoricalStartOn',
  CloseOn = 'closeOn',
  LeErrorNumber = 'leErrorNumber',
  StartOn = 'startOn',
}

@Injectable()
export class ContractsApiMappingService {

  constructor(private apiV3DateService: ApiV3DateService) { }

  public toHttpParams(attributes: HttpParamsAttributes): HttpParams {
    let params = new HttpParams();
    params = this.setFilters(params, attributes.filters);
    return this.setSortParams(params, attributes.sort);
  }

  private setSortParams(params: HttpParams, sortParams: ISortParams): HttpParams {
    if (!sortParams) {
      return params;
    }

    const apiV3SortParams = toApiV3SortParams(sortParams);
    return apiV3SortToHttpParams(params, apiV3SortParams);
  }

  private setFilters(params: HttpParams, filters: IContractsFilter): HttpParams {
    if (!filters) {
      return params;
    }

    params = this.setContractIds(params, filters.ids);
    params = this.setContractName(params, filters.name);
    params = this.setClients(params, filters.clients);
    params = this.setDistributors(params, filters.distributors);
    params = this.setProducts(params, filters.products);
    params = this.setOffers(params, filters.offers);
    params = this.setEnvironments(params, filters.environments);
    params = this.setEstablishments(params, filters.establishments);
    params = this.setContractState(params, filters.states);
    params = this.setEstablishmentState(params, filters.establishmentHealth, filters.environments);
    params = this.setCreatedOn(params, filters.createdAt);
    params = this.setTheoreticalStartOn(params, filters.periodOn?.startDate);
    params = this.setCloseOn(params, filters.periodOn?.endDate);
    return this.setIsDirectSales(params, filters.distributorFilter);
  }

  private setContractIds(params: HttpParams, ids: string[]): HttpParams {
    if (!ids?.length) {
      return params.delete(ContractQueryParamKey.Id);
    }

    return params.set(ContractQueryParamKey.Id, ids.join(','));
  }

  private setContractName(params: HttpParams, name: string): HttpParams {
    if (!name) {
      return params.delete(ContractQueryParamKey.Name);
    }

    return params.set(ContractQueryParamKey.Name, `like,${ encodeURIComponent(name) }`);
  }

  private setClients(params: HttpParams, clients: IClient[]): HttpParams {
    if (!clients?.length) {
      return params.delete(ContractQueryParamKey.ClientId);
    }

    const clientIds = clients.map(u => u.id);
    return params.set(ContractQueryParamKey.ClientId, clientIds.join(','));
  }

  private setDistributors(params: HttpParams, distributors: IDistributor[]): HttpParams {
    if (!distributors?.length) {
      return params.delete(ContractQueryParamKey.DistributorId);
    }

    const distributorIds = distributors.map(u => u.id);
    return params.set(ContractQueryParamKey.DistributorId, distributorIds.join(','));
  }

  private setProducts(params: HttpParams, products: IProduct[]): HttpParams {
    if (!products?.length) {
      return params.delete(ContractQueryParamKey.ProductId);
    }

    const productIds = products.map(u => u.id);
    return params.set(ContractQueryParamKey.ProductId, productIds.join(','));
  }

  private setOffers(params: HttpParams, offers: IOffer[]): HttpParams {
    if (!offers?.length) {
      return params.delete(ContractQueryParamKey.OfferId);
    }

    const offerIds = offers.map(u => u.id);
    return params.set(ContractQueryParamKey.OfferId, offerIds.join(','));
  }

  private setEnvironments(params: HttpParams, environments: IEnvironment[]): HttpParams {
    if (!environments?.length) {
      return params.delete(ContractQueryParamKey.EnvironmentId);
    }

    const environmentIds = environments.map(u => u.id);
    return params.set(ContractQueryParamKey.EnvironmentId, environmentIds.join(','));
  }

  private setEstablishments(params: HttpParams, establishments: IEstablishment[]): HttpParams {
    if (!establishments?.length) {
      return params.delete(ContractQueryParamKey.EstablishmentId);
    }

    const establishmentIds = establishments.map(u => u.id);
    return params.set(ContractQueryParamKey.EstablishmentId, establishmentIds.join(','));
  }

  private setContractState(params: HttpParams, state: IContractState[]): HttpParams {
    if (!state?.length) {
      return params.delete(ContractQueryParamKey.State);
    }

    const stateIds = state.map(u => u.id);
    return params.set(ContractQueryParamKey.State, stateIds.join(','));
  }

  private setIsDirectSales(params: HttpParams, distributorFilter: DistributorFilter): HttpParams {
    if (distributorFilter === DistributorFilter.All) {
      return params.delete(ContractQueryParamKey.IsDirectSales);
    }

    const isDirectSales = distributorFilter === DistributorFilter.Direct;
    return params.set(ContractQueryParamKey.IsDirectSales, isDirectSales.toString());
  }

  private setEstablishmentState(params: HttpParams, etsHealth: IContractEstablishmentHealth, environments: IEnvironment[]): HttpParams {
    const areEnvironmentsFiltered = !!environments?.length;

    if (!etsHealth) {
      params = params
        .delete(ContractQueryParamKey.LeErrorNumber)
        .delete(ContractQueryParamKey.StartOn);

      return areEnvironmentsFiltered ? params : params.delete(ContractQueryParamKey.EnvironmentId);
    }

    switch (etsHealth.id) {
      case ContractEstablishmentHealth.Ok :
        if (!areEnvironmentsFiltered) {
          params = params.set(ContractQueryParamKey.EnvironmentId, 'notequal,null');
        }
        return params.set(ContractQueryParamKey.LeErrorNumber, String(ContractEstablishmentHealth.Ok));
      case ContractEstablishmentHealth.Error :
        return params.set(ContractQueryParamKey.LeErrorNumber, `notequal,${ String(ContractEstablishmentHealth.Ok) }`);
      case ContractEstablishmentHealth.NoEnvironment :
        return params.set(ContractQueryParamKey.EnvironmentId, 'null');
      case ContractEstablishmentHealth.NoEstablishment :
        return params
          .set(ContractQueryParamKey.StartOn, 'null')
          .set(ContractQueryParamKey.LeErrorNumber, String(ContractEstablishmentHealth.Ok))
          .set(ContractQueryParamKey.EnvironmentId, 'notequal,null');
      default:
        return params;
    }
  }

  private setCreatedOn(params: HttpParams, createdOn: Date): HttpParams {
    if (!createdOn) {
      return params.delete(ContractQueryParamKey.CreatedOn);
    }

    const firstOfMonth = new Date(createdOn.getFullYear(), createdOn.getMonth(), 1);
    const lastOfMonth = new Date(createdOn.getFullYear(), createdOn.getMonth() + 1, 0);

    const apiV3CreatedOn = this.apiV3DateService.toApiDateRangeFormat({
      startDate: firstOfMonth,
      endDate: lastOfMonth,
    });
    return params.set(ContractQueryParamKey.CreatedOn, apiV3CreatedOn);
  }

  private setTheoreticalStartOn(params: HttpParams, startAt: Date): HttpParams {
    if (!startAt) {
      return params.delete(ContractQueryParamKey.TheoreticalStartOn);
    }

    const apiV3StartAt = this.apiV3DateService.toApiV3DateFormat(startAt);
    return params.set(ContractQueryParamKey.TheoreticalStartOn, apiV3StartAt);
  }

  private setCloseOn(params: HttpParams, endAt: Date): HttpParams {
    if (!endAt) {
      return params.delete(ContractQueryParamKey.CloseOn);
    }

    const apiV3EndAt = this.apiV3DateService.toApiV3DateFormat(endAt);
    return params.set(ContractQueryParamKey.CloseOn, apiV3EndAt);
  }
}
