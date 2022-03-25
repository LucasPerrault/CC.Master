import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV3CollectionResponse, IHttpApiV3Response } from '@cc/common/queries';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IProduct, productFields } from '../models/product.interface';

@Injectable()
export class ProductsService {

  private readonly productsEndPoint = '/api/v3/products';

  constructor(private httpClient: HttpClient) { }

  public getProductsById$(ids: number[]): Observable<IProduct[]> {
    const params = new HttpParams().set('fields', productFields).set('id', ids.join(','));

    return this.httpClient.get<IHttpApiV3CollectionResponse<IProduct>>(this.productsEndPoint, { params }).pipe(
      map(response => response.data),
      map(data => data.items),
    );
  }

  public getProductById$(id: number): Observable<IProduct> {
    const url = `${ this.productsEndPoint }/${ id }`;
    return this.httpClient.get<IHttpApiV3Response<IProduct>>(url)
      .pipe(map(res => res.data));
  }
}
