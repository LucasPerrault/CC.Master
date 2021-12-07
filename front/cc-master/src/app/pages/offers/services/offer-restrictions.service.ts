import { Injectable } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';
import { IPriceList } from '@cc/domain/billing/offers';
import { isAfter } from 'date-fns';

import { IDetailedOffer } from '../models/detailed-offer.interface';
import { PriceListsTimelineService } from './price-lists-timeline.service';

@Injectable()
export class OfferRestrictionsService {
  constructor(private rightsService: RightsService) {
  }

  public canDeletePriceList(list: IPriceList): boolean {
    const today = Date.now();
    const startDate = new Date(list.startsOn);
    return isAfter(startDate, today) && this.hasRightToCreateOffers();
  }

  public canEdit(offer: IDetailedOffer): boolean {
    return offer.usage?.numberOfCountedContracts === 0 && this.hasRightToCreateOffers();
  }

  public canEditPriceListStartsOn(offer: IDetailedOffer) {
    if (!offer.priceLists || offer?.priceLists?.length === 1) {
      return false;
    }

    const currentPriceList = PriceListsTimelineService.getCurrent(offer.priceLists);
    const sortedPriceLists = offer.priceLists.sort((a, b) =>
      new Date(a.startsOn).getTime() - new Date(b.startsOn).getTime());
    const oldestPriceList = sortedPriceLists[0];

    return oldestPriceList.id !== currentPriceList.id
      && this.canEdit(offer)
      && this.hasRightToCreateOffers();
  }

  public hasRightToCreateOffers(): boolean {
    return this.rightsService.hasOperation(Operation.CreateCommercialOffers);
  }
}
