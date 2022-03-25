import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BYPASS_INTERCEPTOR } from '@cc/aspects/errors';
import { IHttpApiV4CollectionResponse } from '@cc/common/queries';
import { IBillingCoreData } from '@cc/domain/billing/billing-core-data/billing-core-data.interface';
import { IBillingEntity } from '@cc/domain/billing/clients';
import { forkJoin, Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

export const initBillingCoreData = (initializer: BillingCoreDataInitializer): () => Promise<IBillingCoreData> =>
  () => initializer.init().toPromise();
export const getBillingCoreData = (initializer: BillingCoreDataInitializer): IBillingCoreData => initializer.coreData;

@Injectable()
export class BillingCoreDataInitializer {
  coreData: IBillingCoreData;

  private readonly context = new HttpContext().set(BYPASS_INTERCEPTOR, true);

  constructor(private http: HttpClient) {}

  init(): Observable<IBillingCoreData> {
    return forkJoin({
      billingEntities: this.getBillingEntities$(),
    }).pipe(tap(coreData => this.coreData = coreData));
  }

  getBillingEntities$(): Observable<IBillingEntity[]> {

    return this.http
      .get<IHttpApiV4CollectionResponse<IBillingEntity>>('/api/billing-entities', { context: this.context })
      .pipe(
        map(r => r.items),
        catchError(e => of([])),
      );
  }
}
