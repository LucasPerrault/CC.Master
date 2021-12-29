import { Injectable } from '@angular/core';
import { ICount } from '@cc/domain/billing/counts/count.interface';

@Injectable()
export class TimelineCountsService {
  public static getLastCount(counts: ICount[]): ICount {
    if (!counts?.length) {
      return null;
    }

    const sortedDescCounts = counts.sort((a, b) =>
      new Date(b.countPeriod).getTime() - new Date(a.countPeriod).getTime());

    return sortedDescCounts[0];
  }

  public static getLastCountPeriod(counts: ICount[]): Date | null {
    const lastCount = TimelineCountsService.getLastCount(counts);
    return !!lastCount?.countPeriod ? new Date(lastCount.countPeriod) : null;
  }
}
