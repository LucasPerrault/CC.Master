import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { Observable } from 'rxjs';

import { establishmentAdditionalColumns } from '../../../../../establishments/models/establishment-additional-column';
import { FacetScope } from '../../../../models';
import { IFacetAndColumn } from '../facet-and-column.interface';
import { FacetsAndColumnsApiSelectService } from '../facets-and-columns-api-select.service';

@Injectable()
export class EstablishmentFacetsAndColumnsApiSelectService {
  private readonly columns = establishmentAdditionalColumns.map(e => ({ ...e, name: this.translatePipe.transform(e.name) }));

  constructor(private translatePipe: TranslatePipe, private service: FacetsAndColumnsApiSelectService) {}

  public getPaged(page?: number): Observable<IFacetAndColumn[]> {
    return this.service.getPaged(FacetScope.Establishment, this.columns, page);
  }

  public searchPaged(clue?: string, page?: number, filters: string[] = []): Observable<IFacetAndColumn[]> {
   return this.service.searchPaged(FacetScope.Establishment, this.columns, clue, page);
  }
}
