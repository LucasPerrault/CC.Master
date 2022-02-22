import { Injectable } from '@angular/core';
import { IDistributor } from '@cc/domain/billing/distributors/v4';
import { LuApiV4Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

const sortDistributors = (distributors: IDistributor[]) => distributors
  .sort((d1, d2) => d1.name.localeCompare(d2.name));

@Injectable()
export class DistributorApiSelectService extends LuApiV4Service<IDistributor> {
  public getAll(filters?: string[]): Observable<IDistributor[]> {
    return super.getAll(filters).pipe(map(d => sortDistributors(d)));
  }

  public searchAll(clue?: string, filters?: string[]): Observable<IDistributor[]> {
    return super.searchAll(clue, filters).pipe(map(d => sortDistributors(d)));
  }
}
