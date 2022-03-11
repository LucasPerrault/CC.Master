import { Injectable } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

import { ICountsRoutingParams } from './counts-filter-routing.service';


export enum CountsRoutingKey {
  CountPeriod = 'countperiod',
  ClientIds = 'clientids',
  DistributorIds = 'distributorids',
  OfferIds = 'offerids',
  EnvironmentGroupIds = 'environmentgroupids',
  ProductIds = 'productids',
  Columns = 'columns',
  EnvironmentIds = 'environmentids',
}

@Injectable()
export class CountsRoutingService {
  constructor(private activatedRoute: ActivatedRoute, private router: Router) {
  }

  public getRoutingParams(): ICountsRoutingParams {
    const params: ParamMap = this.activatedRoute.snapshot.queryParamMap;
    return {
      countPeriod: params.get(CountsRoutingKey.CountPeriod),
      clientIds: params.get(CountsRoutingKey.ClientIds),
      distributorIds: params.get(CountsRoutingKey.DistributorIds),
      offerIds: params.get(CountsRoutingKey.OfferIds),
      environmentGroupIds: params.get(CountsRoutingKey.EnvironmentGroupIds),
      productIds: params.get(CountsRoutingKey.ProductIds),
      columns: params.get(CountsRoutingKey.Columns),
      environmentIds: params.get(CountsRoutingKey.EnvironmentIds),
    };
  }

  public async updateRouterAsync(params: ICountsRoutingParams): Promise<void> {
    const queryParams = {
      [CountsRoutingKey.CountPeriod]: params.countPeriod,
      [CountsRoutingKey.ClientIds]: params.clientIds,
      [CountsRoutingKey.ProductIds]: params.productIds,
      [CountsRoutingKey.OfferIds]: params.offerIds,
      [CountsRoutingKey.DistributorIds]: params.distributorIds,
      [CountsRoutingKey.EnvironmentGroupIds]: params.environmentGroupIds,
      [CountsRoutingKey.Columns]: params.columns,
      [CountsRoutingKey.EnvironmentIds]: params.environmentIds,
    };

    await this.router.navigate([], {
      queryParams,
      queryParamsHandling: 'merge',
      relativeTo: this.activatedRoute,
    });
  }
}
