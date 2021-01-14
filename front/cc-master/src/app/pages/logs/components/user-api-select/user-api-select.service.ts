import { Injectable } from '@angular/core';
import { IPrincipal } from '@cc/aspects/principal';
import { defaultPagingParams } from '@cc/common/queries';
import { LuApiV3Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class UserApiSelectService extends LuApiV3Service<IPrincipal> {
  private defaultOption = {
    id: 0,
    name: 'CloudControl Admin',
  } as IPrincipal;

  public getAll(filters?: string[]): Observable<IPrincipal[]> {
    return this.getWithDefaultPrincipal$([this.url, ...filters].join('&'), this.defaultOption);
  }

  public getPaged(page: number, filters: string[] = []): Observable<IPrincipal[]> {
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, paging, ...filters].join('&');
    return this.getWithDefaultPrincipal$(url, this.defaultOption);
  }

  public searchAll(clue: string, filters: string[] = []): Observable<IPrincipal[]> {
    if (!clue) {
      return this.getAll(filters);
    }
    const url = [this.url, this._clueFilter(clue), ...filters].join('&');
    return this.getWithDefaultPrincipal$(url, this.defaultOption, clue);
  }

  public searchPaged(clue: string, page: number, filters: string[] = []): Observable<IPrincipal[]> {
    if (!clue) {
      return this.getPaged(page, filters);
    }
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, this._clueFilter(clue), paging, ...filters].join('&');
    return this.getWithDefaultPrincipal$(url, this.defaultOption, clue);
  }

  private getWithDefaultPrincipal$(url: string, defaultOption: IPrincipal, clue?: string): Observable<IPrincipal[]> {
    const principals$ = this._get(url);

    const isClueIncludedInDefaultOption = !!clue && !!defaultOption.name.toLowerCase().includes(clue.toLowerCase());
    if (!clue || isClueIncludedInDefaultOption) {
      return principals$.pipe(map(p => [defaultOption, ...p]));
    }

    return principals$;
  }
}
