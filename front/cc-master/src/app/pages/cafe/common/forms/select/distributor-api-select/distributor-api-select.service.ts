import { Injectable } from '@angular/core';
import { LuApiV4Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IDistributor } from '../../../../environments/models/environment-access.interface';

const sortDistributors = (distributors: IDistributor[]) => distributors
    .sort((d1, d2) => d1.name.localeCompare(d2.name))
    .sort(d => d.isLucca  ? -1 : 1);

@Injectable()
export class DistributorApiSelectService extends LuApiV4Service<IDistributor> {

  public getAll(filters?: string[]): Observable<IDistributor[]> {
    return super.getAll(filters).pipe(map(sortDistributors));
  }

  searchAll(clue?: string, filters?: string[]): Observable<IDistributor[]> {
    return super.searchAll(clue, filters).pipe(map(sortDistributors));
  }
}
