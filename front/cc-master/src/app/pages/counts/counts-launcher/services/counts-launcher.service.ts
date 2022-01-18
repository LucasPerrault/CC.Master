import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IPaginatedResult } from '@cc/common/paging';
import { IContract } from '@cc/domain/billing/contracts';
import { isEqual } from 'date-fns';
import { BehaviorSubject, from, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ICountWithContract } from '../models/count-with-contract.interface';
import { CountsDataService } from './counts-data.service';

interface ICountsStoredByPeriod {
  countPeriod: Date;
  counts: ICountWithContract[];
}

@Injectable()
export class CountsLauncherService {

  private countsStored$: BehaviorSubject<ICountsStoredByPeriod[]> = new BehaviorSubject([]);

  constructor(private dataService: CountsDataService) {}

  public getContractsWithDraftCount$(httpParams: HttpParams, countPeriod: Date): Observable<IPaginatedResult<IContract>> {
    return this.dataService.getDraftCounts$(httpParams, countPeriod)
      .pipe(map(data => ({ items: data.items.map(c => c.contract), totalCount: data.count })));
  }

  public getContractsWithCountWithoutAccountingEntry$(httpParams: HttpParams, countPeriod: Date): Observable<IPaginatedResult<IContract>> {
    return this.dataService.getRealCountsWithoutAccountingEntries(httpParams, countPeriod)
      .pipe(map(data => ({ items: data.items.map(c => c.contract), totalCount: data.count })));
  }

  public getContractsWithoutCount$(countPeriod: Date): Observable<IContract[]> {
    return from(this.getContractsWithoutCountAsync(countPeriod));
  }

  private async getContractsWithoutCountAsync(countPeriod: Date): Promise<IContract[]> {
    if (!this.isAlreadyStored(countPeriod)) {
      const counts = await this.dataService.getCounts$(countPeriod).toPromise();
      this.countsStored$.next([...this.countsStored$.value, { countPeriod, counts }]);
    }

    const countsForThisPeriod = this.getCountsStored(countPeriod)?.counts ?? [];
    const contractIdsShouldBeCount = countsForThisPeriod?.map(c => c.contract.id) ?? [];
    const contracts = await this.dataService.getContracts$(countPeriod).toPromise();
    return contracts?.filter(c => !contractIdsShouldBeCount.includes(c?.id)) ?? [];
  }

  private getCountsStored(countPeriod: Date): ICountsStoredByPeriod {
    return this.countsStored$.value.find(s => isEqual(s.countPeriod, countPeriod));
  }

  private isAlreadyStored(countPeriod: Date): boolean {
    return !!this.getCountsStored(countPeriod);
  }
}
