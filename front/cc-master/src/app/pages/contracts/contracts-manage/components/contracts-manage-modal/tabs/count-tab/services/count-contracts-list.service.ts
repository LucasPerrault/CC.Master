import { Injectable } from '@angular/core';
import { addMonths, isEqual, startOfMonth, subMonths } from 'date-fns';

import { ICountContract } from '../models/count-contract.interface';
import { ICountListEntry } from '../models/count-list-entry.interface';
import { IDetailedCount } from '../models/detailed-count.interface';

@Injectable()
export class CountContractsListService {

  public toCountListEntries(counts: IDetailedCount[], contract: ICountContract): ICountListEntry[] {
      const startPeriod = startOfMonth(new Date(contract.theoricalStartOn));
      const lastCountPeriod = this.getLastCountPeriod(counts);
      const previousMonth = subMonths(new Date(), 1);
      const startOfThisMonth = startOfMonth(new Date());
      const endPeriod = lastCountPeriod.getTime() >= startOfThisMonth.getTime() ? lastCountPeriod : previousMonth;

      const entries = this.getCountListEntries([], startPeriod, endPeriod, counts);
      return entries.sort((a, b) => b.month.getTime() - a.month.getTime());
  }

  private getCountListEntries(
    entries: ICountListEntry[],
    period: Date,
    maxPeriod: Date,
    counts: IDetailedCount[],
  ): ICountListEntry[] {
    if (period.getMonth() > maxPeriod.getMonth() && period.getFullYear() === maxPeriod.getFullYear()) {
      return entries;
    }

    entries = [...entries, this.getCountListEntry(period, counts)];

    const nextPeriod = addMonths(period, 1);
    return this.getCountListEntries(entries, nextPeriod, maxPeriod, counts);
  }

  private getCountListEntry(countPeriod: Date, counts: IDetailedCount[]): ICountListEntry {
    const count = counts.find(c => isEqual(new Date(c?.countPeriod), countPeriod));
    return ({ month: countPeriod, count });
  }

  private getLastCountPeriod(counts: IDetailedCount[]): Date {
    const countsDescSorted = counts.sort((a, b) => new Date(b?.countPeriod).getTime() - new Date(a?.countPeriod).getTime());
    return new Date(countsDescSorted[0]?.countPeriod);
  }
}
