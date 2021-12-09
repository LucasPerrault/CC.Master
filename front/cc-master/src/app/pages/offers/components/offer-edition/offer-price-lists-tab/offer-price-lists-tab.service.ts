import { Injectable } from '@angular/core';
import { IPriceList } from '@cc/domain/billing/offers';
import { isAfter, isBefore, isEqual } from 'date-fns';

export enum PriceListStatus {
  NoDate = 0,
  NotStarted = 1,
  InProgress = 2,
  Finished = 3,
}

@Injectable()
export class OfferPriceListsTabService {
  public getStatus(priceList: IPriceList, nextList: IPriceList): PriceListStatus {
    const today = Date.now();
    const startDate = new Date(priceList.startsOn);
    if (!startDate) {
      return PriceListStatus.NoDate;
    }

    if (!nextList && this.isBeforeOrEqual(startDate, today) || !!nextList && isAfter(new Date(nextList.startsOn), today)) {
      return PriceListStatus.InProgress;
    }

    if (!nextList && isAfter(startDate, today)) {
      return PriceListStatus.NotStarted;
    }

    return PriceListStatus.Finished;
  }

  private isBeforeOrEqual(date: Date | number, dateToCompare: Date | number): boolean {
    return isBefore(date, dateToCompare) || isEqual(date, dateToCompare);
  }
}
