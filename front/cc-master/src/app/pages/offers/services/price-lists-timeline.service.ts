import { IPriceList } from '@cc/domain/billing/offers';
import { compareDesc, isBefore, isEqual } from 'date-fns';

export class PriceListsTimelineService {
  public static get defaultStartsOn(): Date {
    return new Date('01/01/2002');
  }

  public static getCurrent(priceLists: IPriceList[]): IPriceList {
    if (!priceLists?.length) {
      return null;
    }

    const today = Date.now();
    const sortedDescLists = priceLists.sort((a, b) => compareDesc(new Date(a.startsOn), new Date(b.startsOn))) ?? [];
    return sortedDescLists.find(p => isBefore(new Date(p.startsOn), today) || isEqual(new Date(p.startsOn), today));
  }
}
