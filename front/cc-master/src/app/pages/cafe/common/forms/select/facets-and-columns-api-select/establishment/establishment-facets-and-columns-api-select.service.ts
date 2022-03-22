import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { FacetScope } from '../../../../models';
import { IFacetAndColumn } from '../facet-and-column.interface';
import { FacetsAndColumnsApiSelectService } from '../facets-and-columns-api-select.service';

@Injectable()
export class EstablishmentFacetsAndColumnsApiSelectService {
  constructor(private service: FacetsAndColumnsApiSelectService) {
  }

  public getPaged(page?: number): Observable<IFacetAndColumn[]> {
    return this.service.getPaged(FacetScope.Establishment, [], page);
  }

  public searchPaged(clue?: string, page?: number, filters: string[] = []): Observable<IFacetAndColumn[]> {
   return this.service.searchPaged(FacetScope.Establishment, [], clue, page);
  }
}
