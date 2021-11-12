import { Injectable } from '@angular/core';
import { addMonths, isEqual, min, startOfMonth, subMonths } from 'date-fns';

import { ICountContract } from '../models/count-contract.interface';
import { ICountListEntry } from '../models/count-list-entry.interface';
import { IDetailedCount } from '../models/detailed-count.interface';

@Injectable()
export class CountContractsListService {

  public toCountListEntries(counts: IDetailedCount[], contract: ICountContract): ICountListEntry[] {
    const startPeriod = min([startOfMonth(new Date(contract.theoricalStartOn)), this.getFirstCountPeriod(counts)]);
    const endPeriod = this.getEndPeriod(counts, contract.closeOn);

    const entries = this.getCountListEntries(startPeriod, endPeriod, counts);
    return entries.sort((a, b) => b.month.getTime() - a.month.getTime());
  }

  private getCountListEntries(startPeriod: Date, endPeriod: Date, counts: IDetailedCount[]): ICountListEntry[] {
    const entries = [];

    for (let p = startPeriod; p <= endPeriod; p = addMonths(p, 1)) {
      entries.push(this.getCountListEntry(p, counts));
    }

    return entries;
  }

  private getCountListEntry(countPeriod: Date, counts: IDetailedCount[]): ICountListEntry {
    const count = counts.find(c => isEqual(new Date(c?.countPeriod), countPeriod));
    return ({ month: countPeriod, count });
  }

  private getFirstCountPeriod(counts: IDetailedCount[]): Date {
    const countsAscSorted = counts.sort((a, b) => new Date(a?.countPeriod).getTime() - new Date(b?.countPeriod).getTime());
    return new Date(countsAscSorted[0]?.countPeriod);
  }

  private getEndPeriod(counts: IDetailedCount[], closeOn: string): Date {
    const closeDate = !!closeOn ? new Date(closeOn) : null;
    if (!!closeDate) {
      return startOfMonth(closeDate);
    }

    const lastCountPeriod = this.getLastCountPeriod(counts);
    const previousMonth = subMonths(new Date(), 1);
    const startOfThisMonth = startOfMonth(new Date());
    return lastCountPeriod.getTime() >= startOfThisMonth.getTime() ? lastCountPeriod : previousMonth;
  }

  private getLastCountPeriod(counts: IDetailedCount[]): Date {
    const countsDescSorted = counts.sort((a, b) => new Date(b?.countPeriod).getTime() - new Date(a?.countPeriod).getTime());
    return new Date(countsDescSorted[0]?.countPeriod);
  }
}
