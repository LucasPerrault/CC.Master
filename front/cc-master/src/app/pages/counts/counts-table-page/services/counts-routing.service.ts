import { Injectable } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';

import { CountsQueryParamsKey, ICountsQueryParams } from '../models/counts-query-params.interface';

@Injectable()
export class CountsRoutingService {
  constructor(private activatedRoute: ActivatedRoute, private router: Router) {}

  public getRoutingParams(): ICountsQueryParams {
    const params: ParamMap = this.activatedRoute.snapshot.queryParamMap;
    return {
      countPeriod: params.get(CountsQueryParamsKey.CountPeriod),
      clientId: params.get(CountsQueryParamsKey.ClientId),
      distributorId: params.get(CountsQueryParamsKey.DistributorId),
      offerId: params.get(CountsQueryParamsKey.OfferId),
      environmentGroupId: params.get(CountsQueryParamsKey.EnvironmentGroupId),
      productId: params.get(CountsQueryParamsKey.ProductId),
      column: params.get(CountsQueryParamsKey.Column),
      environmentId: params.get(CountsQueryParamsKey.EnvironmentId),
    };
  }

  public async updateRouterAsync(queryParams: ICountsQueryParams): Promise<void> {
    await this.router.navigate([], {
      queryParams,
      queryParamsHandling: 'merge',
      relativeTo: this.activatedRoute,
    });
  }
}
