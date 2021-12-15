import { Injectable } from '@angular/core';
import { DistributorIds } from '@cc/aspects/principal';
import { LuApiV4Service } from '@lucca-front/ng/api';
import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

import { IDistributor } from '../../../../environments/models/environment-access.interface';

@Injectable()
export class DistributorApiSelectService extends LuApiV4Service<IDistributor> {

  public getAll(filters?: string[]): Observable<IDistributor[]> {
    return super.getAll(filters).pipe(this.beginWithLucca);
  }

  searchAll(clue?: string, filters?: string[]): Observable<IDistributor[]> {
    return super.searchAll(clue, filters).pipe(this.beginWithLucca);
  }


  private get beginWithLucca(): UnaryFunction<Observable<IDistributor[]>, Observable<IDistributor[]>> {
    return pipe(
      map(distributors => {
        const luccaDistributor = distributors.find(d => d.id === DistributorIds.lucca);
        if (!luccaDistributor) {
          return distributors;
        }

        const distributorsWithoutLucca = distributors.filter(d => d.id !== DistributorIds.lucca);
        return [luccaDistributor, ...distributorsWithoutLucca];
      }),
    );
  }
}
