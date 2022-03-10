import { Injectable } from '@angular/core';
import { LuApiV4Service } from '@lucca-front/ng/api';
import { Observable } from 'rxjs';

import { FacetValue } from '../../../models/facet-value.interface';

@Injectable()
export class FacetValueApiSelectService extends LuApiV4Service<FacetValue> {
  public searchPaged(clue?: string, page?: number, filters?: string[]): Observable<FacetValue[]> {
    if (!clue) {
      return super.searchPaged(clue, page, filters);
    }

    const urlSafeClues =  encodeURIComponent(clue);
    const searchQuery = `search=${ urlSafeClues }`;
    filters = !!filters?.length ? [...filters, searchQuery] : [searchQuery];
    return super.getPaged(page, filters);
  }
}
