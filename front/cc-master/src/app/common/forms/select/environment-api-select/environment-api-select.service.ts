import { Injectable } from '@angular/core';
import { defaultPagingParams } from '@cc/common/paging';
import { IEnvironment } from '@cc/domain/environments';
import { LuApiV3Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';

@Injectable()
export class EnvironmentApiSelectService extends LuApiV3Service<IEnvironment> {

  public getAll(filters?: string[]): Observable<IEnvironment[]> {
    return this._get([this.url, ...filters].join('&'));
  }

  public getPaged(page: number, filters: string[] = []): Observable<IEnvironment[]> {
    const url = [this.url, this.getPaging(page), ...filters].join('&');
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
    const url = [this.url, this._clueFilter(clue), this.getPaging(page), ...filters].join('&');
    return this._get(url);
  }

  // eslint-disable-next-line @typescript-eslint/naming-convention
  protected _clueFilter(clue: string): string {
    return `subdomain=like,${clue}`;
  }

  private getPaging(page: number): string {
    return `paging=${page * defaultPagingParams.limit},${defaultPagingParams.limit}`;
  }
}
