import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse, IHttpApiV3Response } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { distributorFields, IDistributor } from '../models/distributor.interface';
import { distributorActiveRebateFields, IDistributorActiveRebate } from '../models/distributor-active-rebate.interface';
import { IRebate } from '../models/rebate.interface';

@Injectable()
export class DistributorsService {

  private readonly distributorsEndPoint = '/api/v3/distributors';

  constructor(private httpClient: HttpClient) { }

  public getActiveRebate$(distributorId: number, productId: number): Observable<number> {
    return this.getActiveRebates$(distributorId)
      .pipe(map(rs => rs.find(r => r.productId === productId)?.value));
  }

  public getDistributor$<T extends IDistributor>(id: number, fields: string): Observable<T> {
    const urlById = `${this.distributorsEndPoint}/${id}`;
    const params = new HttpParams().set('fields', fields);

    return this.httpClient.get<IHttpApiV3Response<T>>(urlById, { params })
      .pipe(map(response => response.data));
  }

  public getDistributorsById$(ids: string[]): Observable<IDistributor[]> {
    const params = new HttpParams().set('fields', distributorFields).set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IDistributor>>(this.distributorsEndPoint, { params }).pipe(
      map(response => response.data),
      map(data => data.items),
    );
  }

  private getActiveRebates$(distributorId: number): Observable<IRebate[]> {
    return this.getDistributor$<IDistributorActiveRebate>(distributorId, distributorActiveRebateFields)
      .pipe(map(d => d.activeRebates));
  }
}
