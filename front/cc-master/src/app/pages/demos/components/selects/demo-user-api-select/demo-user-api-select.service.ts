import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { IUser } from '@cc/domain/users/v4';
import { LuApiV4Service } from '@lucca-front/ng/api';
import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { specificAuthors } from '../../../constants/specific-author-id.enum';

@Injectable()
export class DemoUserApiSelectService extends LuApiV4Service<IUser> {

  constructor(http: HttpClient, @Inject(PRINCIPAL) public principal: IPrincipal) {
    super(http);
  }

  public getPaged(page?: number, filters?: string[]): Observable<IUser[]> {
    return super.getPaged(page, filters).pipe(this.toDemoUsers);
  }

  public searchPaged(clue?: string, page?: number, filters?: string[]): Observable<IUser[]> {
    return !!clue ? super.searchPaged(clue, page, filters).pipe(this.toDemoUsers) : this.getPaged(page, filters);
  }

  private get me(): IUser {
    return { id: this.principal.id, firstName: this.principal.name, lastName: '' } as IUser;
  }

  private get specificUsers(): IUser[] {
    return specificAuthors as IUser[];
  }

  private get toDemoUsers(): UnaryFunction<Observable<IUser[]>, Observable<IUser[]>> {
    return pipe(this.excludePrincipal, map(users => [this.me, ...this.specificUsers, ...users]));
  }

  private get excludePrincipal(): UnaryFunction<Observable<IUser[]>, Observable<IUser[]>> {
    return pipe(map(users => users.filter(u => u.id !== this.me.id)));
  }
}
