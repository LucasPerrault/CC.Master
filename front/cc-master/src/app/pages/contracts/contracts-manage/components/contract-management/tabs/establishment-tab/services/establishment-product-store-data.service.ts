import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse } from '@cc/common/queries';
import { IProduct, ISolution } from '@cc/domain/billing/offers';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface ISolutionProduct extends IProduct {
  solutions: ISolution[];
}

@Injectable()
export class EstablishmentProductStoreDataService {

  constructor(private httpClient: HttpClient) {
  }

  public getProducts$(ids: number[]): Observable<ISolutionProduct[]> {
    const url = '/api/v3/products';
    const params = new HttpParams()
      .set('id', ids.join(','))
      .set('fields', 'id,name,solutions[id,name]');

    return this.httpClient.get<IHttpApiV3CollectionResponse<ISolutionProduct>>(url, { params })
      .pipe(map(response => response.data.items));
  }
}
