import { Injectable } from '@angular/core';
import { defaultPagingParams } from '@cc/common/paging';
import { cloudControlAdmin, IUser } from '@cc/domain/users';
import { LuApiV3Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class UserApiSelectService extends LuApiV3Service<IUser> {

  public getAll(filters?: string[]): Observable<IUser[]> {
    const url = [this.url, ...filters].join('&');
    return this.get$(url);
  }

  public getPaged(page: number, filters: string[] = []): Observable<IUser[]> {
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, paging, ...filters].join('&');
    return this.get$(url);
  }

  public searchAll(clue: string, filters: string[] = []): Observable<IUser[]> {
    if (!clue) {
      return this.getAll(filters);
    }
    const url = [this.url, this._clueFilter(clue), ...filters].join('&');
    return this.getWithSearch$(url, clue);
  }

  public searchPaged(clue: string, page: number, filters: string[] = []): Observable<IUser[]> {
    if (!clue) {
      return this.getPaged(page, filters);
    }
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, this._clueFilter(clue), paging, ...filters].join('&');
    return this.getWithSearch$(url, clue);
  }

  private getWithSearch$(url: string, clue: string): Observable<IUser[]> {
    const users$ = this.get$(url);
    return users$.pipe(map(user =>
      user.filter(u => u.name.toLowerCase().includes(clue.toLowerCase()))),
    );
  }

  private get$(url: string): Observable<IUser[]> {
    return this.shouldBeIncludedUserAdmin(this._filters)
      ? this.getWithUserAdmin$(url)
      : this._get(url);
  }

  private getWithUserAdmin$(url: string): Observable<IUser[]> {
    return this._get(url).pipe(map(u => [cloudControlAdmin, ...u]));
  }

  private shouldBeIncludedUserAdmin(filters: string[]): boolean {
    if (!filters.length) {
      return true;
    }

    const queryWithIdsExcluded = filters.find(f => f.includes('id=notequal'));
    if (!queryWithIdsExcluded) {
      return true;
    }

    const idsExcluded = queryWithIdsExcluded.split(',').slice(1);
    return !idsExcluded.includes(cloudControlAdmin.id.toString());
  }
}
