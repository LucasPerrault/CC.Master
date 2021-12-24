import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';

import { EstablishmentProductStoreDataService,ISolutionProduct } from './establishment-product-store-data.service';

@Injectable()
export class EstablishmentProductStoreService {

  private products$: BehaviorSubject<ISolutionProduct[]> = new BehaviorSubject<ISolutionProduct[]>([]);

  constructor(private dataService: EstablishmentProductStoreDataService) {
  }

  public getProducts$(ids: number[]): Observable<ISolutionProduct[]> {
    const allProductIds = this.products$.value?.map(p => p.id);
    const missingProductIds = ids.filter(id => !allProductIds.includes(id));

    if (!missingProductIds.length) {
      return of(this.getProducts(ids));
    }

    return this.dataService.getProducts$(missingProductIds).pipe(
      tap(products => this.add(products)),
      map(() => this.getProducts(ids)),
    );
  }

  private getProducts(ids: number[]): ISolutionProduct[] {
    return this.products$.value?.filter(u => ids.includes(u.id)) ?? [];
  }

  private add(products: ISolutionProduct[]): void {
    this.products$.next([...this.products$.value, ...products]);
  }
}
