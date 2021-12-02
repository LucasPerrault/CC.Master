import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { IOfferUsage } from '../models/offer-usage.interface';
import { OffersDataService } from './offers-data.service';

interface IOfferStored {
  offerId: number;
  usage: IOfferUsage;
}

@Injectable()
export class OfferUsageStoreService {
  private offersStored: BehaviorSubject<IOfferStored[]> = new BehaviorSubject<IOfferStored[]>([]);

  constructor(private dataService: OffersDataService) {
  }

  public getUsage$(offerId: number): Observable<IOfferUsage> {
    return this.getUsages$([offerId]).pipe(map(usages => usages[0]));
  }

  public getUsages$(offerIds: number[]): Observable<IOfferUsage[]> {
    const allOfferIdsWithUsage = this.offersStored.value?.map(store => store.offerId);
    const offerIdsWithoutUsage = offerIds.filter(offerId => !allOfferIdsWithUsage.includes(offerId));

    if (!offerIdsWithoutUsage.length) {
      return of(this.getUsages(offerIds));
    }

    return this.dataService.getUsages$(offerIdsWithoutUsage).pipe(
      tap(usages => this.add(offerIds, usages)),
      map(() => this.getUsages(offerIds)),
    );
  }

  private getUsages(offerIds: number[]): IOfferUsage[] {
    const offers = this.offersStored.value?.filter(s => offerIds.includes(s.offerId)) ?? [];
    return offers.map(o => o.usage);
  }

  private add(offerIds: number[], usages: IOfferUsage[]): void {
    const offersToStore = offerIds.map(offerId => this.toOfferStored(offerId, usages));
    this.offersStored.next([...this.offersStored.value, ...offersToStore]);
  }

  private toOfferStored(offerId: number, usages: IOfferUsage[]): IOfferStored {
    const usage = usages.find(u => u.offerId === offerId);
    return { offerId, usage };
  }
}
