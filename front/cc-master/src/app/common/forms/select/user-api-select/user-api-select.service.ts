import { Injectable } from '@angular/core';
import { defaultPagingParams } from '@cc/common/paging';
import { cloudControlAdmin, IUser } from '@cc/domain/users';
import { LuApiV3Service } from '@lucca-front/ng/api';
import { Observable, pipe, UnaryFunction } from 'rxjs';
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
    const hardcodedUsers = this.getHardcodedUsers();
    return this._get(url).pipe(startWithHardcodedUsers(hardcodedUsers));
  }


  // CloudControl Admin is not in api, but is needed as a selectable user.
  // As a result, we'll add it front-side at the end of the request.
  // The following methods handle this unusual need.

  private getHardcodedUsers() {
    const hardcodedUsers = [];
    if (this.shouldIncludeUserAdmin(this._filters)) {
      hardcodedUsers.push(cloudControlAdmin);
    }
    return hardcodedUsers;
  }

  private shouldIncludeUserAdmin(filters: string[]): boolean {
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

const startWithHardcodedUsers = (users: IUser[]): UnaryFunction<Observable<IUser[]>, Observable<IUser[]>> =>
  pipe(map(u => [...users, ...u]));
