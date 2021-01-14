import { defaultPagingParams } from '@cc/common/queries';
import { LuApiV3Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';

import { IEnvironment } from '../../models';

export class EnvironmentApiSelectService extends LuApiV3Service<IEnvironment> {

  public getAll(filters?: string[]): Observable<IEnvironment[]> {
    return this._get([this.url, ...filters].join('&'));
  }

  public getPaged(page: number, filters: string[] = []): Observable<IEnvironment[]> {
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, paging, ...filters].join('&');
    return this._get(url);
  }

  public searchAll(clue: string, filters: string[] = []): Observable<IEnvironment[]> {
    if (!clue) {
      return this.getAll(filters);
    }
    const url = [this.url, this._clueFilter(clue), ...filters].join('&');
    return this._get(url);
  }

  public searchPaged(clue: string, page: number, filters: string[] = []): Observable<IEnvironment[]> {
    if (!clue) {
      return this.getPaged(page, filters);
    }
    const paging = `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
    const url = [this.url, this._clueFilter(clue), paging, ...filters].join('&');
    return this._get(url);
  }

  // eslint-disable-next-line @typescript-eslint/naming-convention
  protected _clueFilter(clue: string): string {
    return `subdomain=like,${clue}`;
  }
}
