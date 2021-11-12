import { Injectable } from '@angular/core';
import { IPriceList } from '@cc/domain/billing/offers';

import { OfferPriceListService } from '../../services/offer-price-list.service';
import { IOfferEditionValidationContext } from './offer-edition-validation-context.interface';

@Injectable()
export class OfferEditionRestrictionsService {
  public canEdit(context: IOfferEditionValidationContext): boolean {
    return context.realCountNumber === 0;
  }

  public canEditPriceListStartsOn(context: IOfferEditionValidationContext) {
    if (context.offer?.priceLists?.length === 1) {
      return false;
    }

    const currentPriceList = OfferPriceListService.getCurrent(context.offer.priceLists);
    const oldestPriceList = this.getOldestPriceList(context.offer.priceLists);
    return this.canEdit(context) && oldestPriceList.id !== currentPriceList.id;
  }

  private getOldestPriceList(priceLists: IPriceList[]): IPriceList {
    const sortedPriceLists = priceLists.sort((a, b) =>
      new Date(a.startsOn).getTime() - new Date(b.startsOn).getTime());

    return sortedPriceLists[0];
  }
}
