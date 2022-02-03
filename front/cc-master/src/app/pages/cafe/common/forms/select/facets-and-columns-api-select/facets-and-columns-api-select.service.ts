import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IHttpApiV4CollectionCountResponse } from '@cc/common/queries';
import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { FacetScope, IAdditionalColumn, IFacet } from '../../../models';
import { FacetAndColumnHelper, IFacetAndColumn } from './facet-and-column.interface';

@Injectable()
export class FacetsAndColumnsApiSelectService {
  constructor(private http: HttpClient) {}

  public getPaged(scope: FacetScope, columns: IAdditionalColumn[], page?: number): Observable<IFacetAndColumn[]> {
    const params = this.toHttpParams(page);
    return this.get$(scope, params).pipe(this.beginWith(columns, page === 0));
  }

  public searchPaged(scope: FacetScope, columns: IAdditionalColumn[], clue: string, page: number): Observable<IFacetAndColumn[]> {
    if (!clue) {
      return this.getPaged(scope, columns, page);
    }
    const params = this.toHttpParams(page, clue);
    const filteredColumns = columns.filter(column => column?.name?.toLowerCase().includes(clue.toLowerCase()));
    return this.get$(scope, params).pipe(this.beginWith(filteredColumns, page === 0));
  }

  private get$(scope: FacetScope, params: HttpParams): Observable<IFacetAndColumn[]> {
    return this.http.get<IHttpApiV4CollectionCountResponse<IFacet>>(this.getApi(scope), { params })
      .pipe(map(res => res.items), FacetAndColumnHelper.mapFacetsToFacetAndColumns);
  }

  private beginWith(columns: IAdditionalColumn[], condition: boolean):
    UnaryFunction<Observable<IFacetAndColumn[]>, Observable<IFacetAndColumn[]>> {
    return pipe(map(o => condition ? [...FacetAndColumnHelper.mapColumnsToFacetAndColumns(columns), ...o] : o));
  }

  private toHttpParams(page: number, clue?: string): HttpParams {
    let params = new HttpParams()
      .set('fields.root', 'count')
      .set('page', `${ page + 1}`);

    if (!!clue) {
      params = params.set('search', `${ clue.split(' ').map(c => encodeURIComponent(c)) }`);
    }

    return params;
  }

  private getApi(scope: FacetScope): string {
    switch (scope) {
      case FacetScope.Environment:
        return '/api/cafe/facets/environments';
      case FacetScope.Establishment:
        return '/api/cafe/facets/establishments';
    }
  }
}
