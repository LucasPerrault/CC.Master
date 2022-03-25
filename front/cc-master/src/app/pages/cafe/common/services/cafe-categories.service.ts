import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';

import { CafeCategory } from '../enums/cafe-category.enum';


@Injectable()
export class CafeCategoriesService {

  public get category$(): Observable<CafeCategory> {
    return this.category.asObservable();
  }

  private category = new ReplaySubject<CafeCategory>(1);

  public update(category: CafeCategory): void {
    this.category.next(category);
  }
}
