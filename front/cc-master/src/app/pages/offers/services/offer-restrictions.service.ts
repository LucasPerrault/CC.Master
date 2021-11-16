import { Injectable } from '@angular/core';
import { Operation, RightsService } from '@cc/aspects/rights';
import { IPriceList } from '@cc/domain/billing/offers';
import { isAfter } from 'date-fns';

import { IOfferValidationContext } from '../models/offer-validation-context.interface';
import { PriceListsTimelineService } from './price-lists-timeline.service';

@Injectable()
export class OfferRestrictionsService {
  constructor(private rightsService: RightsService) {
  }

  public canDeleteOffer(activeContractNumber: number): boolean {
    return activeContractNumber === 0 && this.hasRightToCreateOffers();
  }

  public canDeletePriceList(list: IPriceList): boolean {
    const today = Date.now();
    const startDate = new Date(list.startsOn);
    return isAfter(startDate, today) && this.hasRightToCreateOffers();
  }

  public canEdit(context: IOfferValidationContext): boolean {
    return context.realCountNumber === 0 && this.hasRightToCreateOffers();
  }

  public canEditPriceListStartsOn(context: IOfferValidationContext) {
    if (context.offer?.priceLists?.length === 1) {
      return false;
    }

    const currentPriceList = PriceListsTimelineService.getCurrent(context.offer.priceLists);
    const sortedPriceLists = context.offer.priceLists.sort((a, b) =>
      new Date(a.startsOn).getTime() - new Date(b.startsOn).getTime());
    const oldestPriceList = sortedPriceLists[0];

    return oldestPriceList.id !== currentPriceList.id
      && this.canEdit(context)
      && this.hasRightToCreateOffers();
  }

  public hasRightToCreateOffers(): boolean {
    return this.rightsService.hasOperation(Operation.CreateCommercialOffers);
  }
}
