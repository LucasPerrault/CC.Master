import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IDistributor } from '@cc/domain/billing/distributors/v4';

import { IDemoAuthor } from '../models/demo.interface';
import { IDemoFilters } from '../models/demo-filters.interface';

enum DemosQueryParamKey {
  Search = 'search',
  AuthorId = 'authorId',
  DistributorId = 'distributorId',
  IsProtected = 'instance.isProtected',
}

@Injectable()
export class DemosApiMappingService {

  public toHttpParams(filters: IDemoFilters): HttpParams {
    let params = new HttpParams();
    params = this.setSearchHttpParams(params, filters.search);
    params = this.setAuthorHttpParams(params, filters.author);
    params = this.setDistributorHttpParams(params, filters.distributor);
    return this.setIsProtectedHttpParams(params, filters.isProtected);
  }

  private setSearchHttpParams(params: HttpParams, search: string) {
    if (!search) {
      return params.delete(DemosQueryParamKey.Search);
    }

    const urlSafeClues = search.trim().split(' ');
    return params.set(DemosQueryParamKey.Search, `${ urlSafeClues }`);
  }

  private setAuthorHttpParams(params: HttpParams, author: IDemoAuthor) {
    return !!author
      ? params.set(DemosQueryParamKey.AuthorId, author.id)
      : params.delete(DemosQueryParamKey.AuthorId);
  }

  private setDistributorHttpParams(params: HttpParams, distributor: IDistributor) {
    return !!distributor
      ? params.set(DemosQueryParamKey.DistributorId, distributor.id)
      : params.delete(DemosQueryParamKey.DistributorId);
  }

  private setIsProtectedHttpParams(params: HttpParams, isProtected: boolean) {
    return isProtected
      ? params.set(DemosQueryParamKey.IsProtected, `${ isProtected }`)
      : params.delete(DemosQueryParamKey.IsProtected);
  }
}
