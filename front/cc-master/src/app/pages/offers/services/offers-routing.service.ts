import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { IOfferQueryParams, OffersQueryParamsKey } from '../models/offer-query-params.interface';

@Injectable()
export class OffersRoutingService {
  constructor(private activatedRoute: ActivatedRoute, private router: Router) {}

  public getRoutingParams(): IOfferQueryParams {
    const params = this.activatedRoute.snapshot.queryParamMap;
    return {
      search: params.get(OffersQueryParamsKey.Search),
      tag : params.get(OffersQueryParamsKey.Tag),
      productId: params.get(OffersQueryParamsKey.ProductId),
      currency: params.get(OffersQueryParamsKey.Currency),
      billingMode: params.get(OffersQueryParamsKey.BillingMode),
      state: params.get(OffersQueryParamsKey.State),
    };
  }

  public setDefaultTag(): void {
    const routingParams = this.getRoutingParams();
    if (!!routingParams.tag) {
      return;
    }

    // It is defined in the offer model in the back project.
    // It is used for the default selection.
    const defaultTag = 'Catalogues';
    routingParams.tag = defaultTag;
    this.updateRouterAsync(routingParams);
  }

  public async updateRouterAsync(queryParams: IOfferQueryParams): Promise<void> {
    await this.router.navigate([], {
      queryParams,
      queryParamsHandling: 'merge',
      relativeTo: this.activatedRoute,
    });
  }
}
