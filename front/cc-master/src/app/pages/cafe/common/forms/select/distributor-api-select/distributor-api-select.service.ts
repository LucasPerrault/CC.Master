import { Injectable } from '@angular/core';
import { DistributorIds } from '@cc/aspects/principal';
import { LuApiV4Service } from '@lucca-front/ng/api';
import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { IDistributor } from '../../../../environments/models/environment-access.interface';

const hardcodedLuccaDistributor: IDistributor = {
  id: DistributorIds.lucca,
  name: 'Lucca',
};

@Injectable()
export class DistributorApiSelectService extends LuApiV4Service<IDistributor> {

  public getPaged(page?: number, filters?: string[]): Observable<IDistributor[]> {
    const pagedWithoutLucca$ = super.getPaged(page, filters).pipe(this.excludeLucca);

    return this.isFirstPage(page) ? pagedWithoutLucca$.pipe(this.beginWithLucca) : pagedWithoutLucca$;
  }

  searchPaged(clue?: string, page?: number, filters?: string[]): Observable<IDistributor[]> {
    const searchPagedWithoutLucca$ = super.searchPaged(clue, page, filters).pipe(this.excludeLucca);

    return this.isFirstPage(page)
      ? searchPagedWithoutLucca$.pipe(this.beginWithLucca, map(d => this.filterByName(d, clue)))
      : searchPagedWithoutLucca$;
  }

  private isFirstPage(page: number): boolean {
    return page === 0;
  }

  private get beginWithLucca(): UnaryFunction<Observable<IDistributor[]>, Observable<IDistributor[]>> {
    return pipe(map(distributors => [hardcodedLuccaDistributor, ...distributors]));
  }

  private get excludeLucca(): UnaryFunction<Observable<IDistributor[]>, Observable<IDistributor[]>> {
    return pipe(map(distributors => distributors.filter(d => d.id !== DistributorIds.lucca)));
  }

  private filterByName = (distributors: IDistributor[], clue: string): IDistributor[] =>
    !!clue ? distributors.filter(d => d.name.toLowerCase().includes(clue.toLowerCase())) : distributors;
}
