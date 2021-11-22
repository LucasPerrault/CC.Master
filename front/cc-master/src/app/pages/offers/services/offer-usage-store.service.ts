import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

import { IOfferUsage } from '../models/offer-usage.interface';
import { OffersDataService } from './offers-data.service';
import { map, tap } from 'rxjs/operators';

@Injectable()
export class OfferUsageStoreService {
  private usages$: BehaviorSubject<IOfferUsage[]> = new BehaviorSubject<IOfferUsage[]>([]);

  constructor(private dataService: OffersDataService) {
  }

  public getUsage$(offerId: number): Observable<IOfferUsage> {
    return this.getUsages$([offerId]).pipe(map(usages => usages[0]));
  }

  public getUsages$(offerIds: number[]): Observable<IOfferUsage[]> {
    const allOfferIdsWithUsage = this.usages$.value?.map(u => u.offerId);
    const offerIdsWithoutUsage = offerIds.filter(offerId => !allOfferIdsWithUsage.includes(offerId));

    return this.dataService.getUsages$(offerIdsWithoutUsage).pipe(
      tap(usages => this.add(usages)),
      map(() => this.getUsages(offerIds)),
    );
  }

  private getUsages(offerIds: number[]): IOfferUsage[] {
    return this.usages$.value?.filter(u => offerIds.includes(u.offerId)) ?? [];
  }

  private add(usages: IOfferUsage[]): void {
    this.usages$.next([...this.usages$.value, ...usages]);
  }
}
