import { Injectable } from '@angular/core';
import { LuApiV3Service } from '@lucca-front/ng/api';
import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { IDemoInstanceUser } from './demo-instance-user.interface';

@Injectable()
export class DemoInstanceUserApiSelectService extends LuApiV3Service<IDemoInstanceUser> {

  private readonly mainInstanceUserIds = [16, 15, 35];
  private readonly instanceUsersLimit = 100;

  public getAll(filters: string[] = []): Observable<IDemoInstanceUser[]> {
    filters = [`paging=0,${ this.instanceUsersLimit }`, ...filters];
    return super.getAll(filters).pipe(this.sortMainUsers);
  }

  public searchAll(clue: string, filters: string[] = []): Observable<IDemoInstanceUser[]> {
    if (!clue) {
      return this.getAll(filters);
    }

    filters = [`paging=0,${ this.instanceUsersLimit }`, ...filters];
    return super.searchAll(clue, filters).pipe(this.sortMainUsers);
  }

  // eslint-disable-next-line @typescript-eslint/naming-convention
  protected _clueFilter(clue: string) {
    const urlSafeClue = encodeURIComponent(clue);
    return `clue=${urlSafeClue}`;
  }

  private get sortMainUsers(): UnaryFunction<Observable<IDemoInstanceUser[]>, Observable<IDemoInstanceUser[]>> {
    return pipe(map(users => users.sort((a, b) => this.mainInstanceUserIds.indexOf(b.id) - this.mainInstanceUserIds.indexOf(a.id))));
  }
}
