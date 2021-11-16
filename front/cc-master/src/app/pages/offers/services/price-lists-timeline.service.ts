import { IPriceList } from '@cc/domain/billing/offers';
import { isBefore, isEqual } from 'date-fns';

export class PriceListsTimelineService {
  public static get defaultStartsOn(): Date {
    return new Date('01/01/2002');
  }

  public static getCurrent(priceLists: IPriceList[]): IPriceList {
    const today = Date.now();
    return priceLists.find(p => isBefore(new Date(p.startsOn), today) || isEqual(new Date(p.startsOn), today));
  }
}
