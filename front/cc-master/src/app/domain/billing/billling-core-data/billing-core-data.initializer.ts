import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionResponse } from '@cc/common/queries';
import { IBillingCoreData } from '@cc/domain/billing/billling-core-data/billing-core-data.interface';
import { IBillingEntity } from '@cc/domain/billing/clients';
import { forkJoin, Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

export const initBillingCoreData = (initializer: BillingCoreDataInitializer): () => Promise<IBillingCoreData> =>
  () => initializer.init().toPromise();
export const getBillingCoreData = (initializer: BillingCoreDataInitializer): IBillingCoreData => initializer.coreData;

@Injectable()
export class BillingCoreDataInitializer {
  coreData: IBillingCoreData;
  constructor(private http: HttpClient) {}

  init(): Observable<IBillingCoreData> {
    return forkJoin({
      billingEntities: this.http.get<IHttpApiV4CollectionResponse<IBillingEntity>>('/api/billing-entities').pipe(map(r => r.items)),
    }).pipe(tap(coreData => this.coreData = coreData));
  }
}
