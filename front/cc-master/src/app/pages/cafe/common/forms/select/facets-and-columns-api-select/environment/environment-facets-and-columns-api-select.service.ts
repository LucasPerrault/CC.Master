import { Injectable } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { Observable } from 'rxjs';

import { environmentAdditionalColumns } from '../../../../../environments/models/environment-additional-column';
import { FacetScope } from '../../../../models';
import { IFacetAndColumn } from '../facet-and-column.interface';
import { FacetsAndColumnsApiSelectService } from '../facets-and-columns-api-select.service';

@Injectable()
export class EnvironmentFacetsAndColumnsApiSelectService {
  private readonly columns = environmentAdditionalColumns.map(e => ({ ...e, name: this.translatePipe.transform(e.name) }));

  constructor(
    private service: FacetsAndColumnsApiSelectService,
    private translatePipe: TranslatePipe,
  ) {}

  public getPaged(page?: number): Observable<IFacetAndColumn[]> {
    return this.service.getPaged(FacetScope.Environment, this.columns, page);
  }

  public searchPaged(clue?: string, page?: number, filters: string[] = []): Observable<IFacetAndColumn[]> {
   return this.service.searchPaged(FacetScope.Environment, this.columns, clue, page);
  }
}
