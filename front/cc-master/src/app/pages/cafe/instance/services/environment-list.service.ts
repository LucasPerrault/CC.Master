import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PagingService } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IEnvironment } from '../models/environment.interface';
import { EnvironmentDataService } from './environment-data.service';

@Injectable()
export class EnvironmentListService {
  constructor(private pagingService: PagingService, private environmentsService: EnvironmentDataService) {
  }

  public getPaginatedEnvironments$(): PaginatedList<IEnvironment> {
    return this.pagingService.paginate<IEnvironment>(
      (httpParams) => this.getEnvironments$(httpParams),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getEnvironments$(httpParams: HttpParams): Observable<IPaginatedResult<IEnvironment>> {
    return this.environmentsService.getEnvironments$(httpParams).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }

}
