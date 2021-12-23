import { Injectable } from '@angular/core';
import { LuApiV4Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';

import { IDistributor } from '../../../../environments/models/environment-access.interface';

@Injectable()
export class DistributorApiSelectService extends LuApiV4Service<IDistributor> {

  public getAll(filters?: string[]): Observable<IDistributor[]> {
    return super.getAll([...filters, 'sort=-isLucca,name']);
  }

  searchAll(clue?: string, filters?: string[]): Observable<IDistributor[]> {
    return super.searchAll(clue, [...filters, 'sort=-isLucca,name']);
  }
}
