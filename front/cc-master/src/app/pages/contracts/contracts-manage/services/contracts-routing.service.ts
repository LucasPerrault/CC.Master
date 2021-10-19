import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { IContractsRoutingParams } from '../models/contracts-routing-params.interface';

enum ContractsRoutingKey {
  Ids = 'contractIds',
  Name = 'name',
  State = 'stateids',
  EstablishmentState = 'legalentitiestypeids',
  IsDirectSales = 'isdirectsales',
  CreatedAt = 'creationdate',
  StartAt = 'startdate',
  EndAt = 'enddate',
  ClientIds = 'clientids',
  OfferIds = 'offerids',
  ProductIds = 'productids',
  EnvironmentIds = 'environmentids',
  DistributorIds = 'distributorids',
  EstablishmentIds = 'legalentityids',
  Columns = 'columns',
}

@Injectable()
export class ContractsRoutingService {
  constructor(private activatedRoute: ActivatedRoute, private router: Router) {
  }

  public getContractsRoutingParams(): IContractsRoutingParams {
    const params = this.activatedRoute.snapshot.queryParamMap;

    return {
      ids: params.get(ContractsRoutingKey.Ids),
      name: params.get(ContractsRoutingKey.Name),
      states: params.get(ContractsRoutingKey.State),
      establishmentHealth: params.get(ContractsRoutingKey.EstablishmentState),
      isDirectSales: params.get(ContractsRoutingKey.IsDirectSales),
      createdAt: params.get(ContractsRoutingKey.CreatedAt),
      startAt: params.get(ContractsRoutingKey.StartAt),
      endAt: params.get(ContractsRoutingKey.EndAt),
      clientIds: params.get(ContractsRoutingKey.ClientIds),
      productIds: params.get(ContractsRoutingKey.ProductIds),
      offerIds: params.get(ContractsRoutingKey.OfferIds),
      distributorIds: params.get(ContractsRoutingKey.DistributorIds),
      environmentIds: params.get(ContractsRoutingKey.EnvironmentIds),
      establishmentIds: params.get(ContractsRoutingKey.EstablishmentIds),
      columns: params.get(ContractsRoutingKey.Columns),
    };
  }

  public async updateRouterAsync(params: IContractsRoutingParams): Promise<void> {
    const queryParams = {
      [ContractsRoutingKey.Ids]: params.ids,
      [ContractsRoutingKey.Name]: params.name,
      [ContractsRoutingKey.State]: params.states,
      [ContractsRoutingKey.EstablishmentState]: params.establishmentHealth,
      [ContractsRoutingKey.IsDirectSales]: params.isDirectSales,
      [ContractsRoutingKey.CreatedAt]: params.createdAt,
      [ContractsRoutingKey.StartAt]: params.startAt,
      [ContractsRoutingKey.EndAt]: params.endAt,
      [ContractsRoutingKey.ClientIds]: params.clientIds,
      [ContractsRoutingKey.ProductIds]: params.productIds,
      [ContractsRoutingKey.OfferIds]: params.offerIds,
      [ContractsRoutingKey.DistributorIds]: params.distributorIds,
      [ContractsRoutingKey.EnvironmentIds]: params.environmentIds,
      [ContractsRoutingKey.EstablishmentIds]: params.establishmentIds,
      [ContractsRoutingKey.Columns]: params.columns,
    };

    await this.router.navigate([], {
      queryParams,
      queryParamsHandling: 'merge',
      relativeTo: this.activatedRoute,
    });
  }

  public async redirectToStandardParamsIfNecessary(): Promise<void> {
    if (hasLegacyParams(this.router.url)) {
      const queryParams = getLegacyParams(this.router.url);
      await this.router.navigate([getUrlWithoutParams(this.router.url)], { queryParams });
    }
    return Promise.resolve();
  }

}
