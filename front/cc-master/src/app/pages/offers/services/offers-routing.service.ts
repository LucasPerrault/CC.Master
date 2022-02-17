import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { IOfferRoutingParams } from './offers-filter-routing.service';

enum OffersRoutingKey {
  Search = 'search',
  Tag = 'tag',
  ProductId = 'productId',
  Currencies = 'currencies',
  BillingModes = 'billingModes',
  State = 'state'
}

@Injectable()
export class OffersRoutingService {
  constructor(private activatedRoute: ActivatedRoute, private router: Router) {}

  public getRoutingParams(): IOfferRoutingParams {
    const params = this.activatedRoute.snapshot.queryParamMap;
    return {
      search: params.get(OffersRoutingKey.Search),
      tag : params.get(OffersRoutingKey.Tag),
      productId: params.get(OffersRoutingKey.ProductId),
      currencies: params.get(OffersRoutingKey.Currencies),
      billingModes: params.get(OffersRoutingKey.BillingModes),
      state: params.get(OffersRoutingKey.State),
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

  public async updateRouterAsync(routingParams: IOfferRoutingParams): Promise<void> {
    const queryParams = {
      [OffersRoutingKey.Search]: routingParams.search,
      [OffersRoutingKey.Tag]: routingParams.tag,
      [OffersRoutingKey.ProductId]: routingParams.productId,
      [OffersRoutingKey.Currencies]: routingParams.currencies,
      [OffersRoutingKey.BillingModes]: routingParams.billingModes,
      [OffersRoutingKey.State]: routingParams.state,
    };

    await this.router.navigate([], {
      queryParams,
      queryParamsHandling: 'merge',
      relativeTo: this.activatedRoute,
    });
  }
}
