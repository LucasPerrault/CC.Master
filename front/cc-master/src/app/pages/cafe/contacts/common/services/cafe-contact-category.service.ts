import { Injectable } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';

import { ContactCategory } from '../enums/cafe-contacts-category.enum';

@Injectable()
export class CafeContactCategoryService {

  public get category$(): Observable<ContactCategory> {
    return this.category.asObservable();
  }

  private category = new ReplaySubject<ContactCategory>(1);

  public update(category: ContactCategory): void {
    this.category.next(category);
  }
}
