import { HttpParams } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IEnvironment } from './models/environment.interface';
import { EnvironmentDataService } from './services/environment-data.service';

@Component({
  selector: 'cc-cafe-instances',
  templateUrl: './cafe-instances.component.html',
})
export class CafeInstancesComponent {
  public get environments$(): Observable<IEnvironment[]> {
    return this.paginatedEnvironments.items$;
  }

  public get environmentsCount$(): Observable<number> {
    return this.paginatedEnvironments.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedEnvironments.state$
      .pipe(map(state => state === PaginatedListState.Update));
  }

  public selectedColumns: FormControl = new FormControl([]);

  private paginatedEnvironments: PaginatedList<IEnvironment>;

  constructor(private pagingService: PagingService, private environmentsDataService: EnvironmentDataService) {
    this.paginatedEnvironments = this.getPaginatedEnvironments$();
  }

  public nextPage(): void {
    this.paginatedEnvironments.nextPage();
  }

  public getPaginatedEnvironments$(): PaginatedList<IEnvironment> {
    return this.pagingService.paginate<IEnvironment>(
      (httpParams) => this.getEnvironments$(httpParams),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  private getEnvironments$(httpParams: HttpParams): Observable<IPaginatedResult<IEnvironment>> {
    return this.environmentsDataService.getEnvironments$(httpParams).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }
}
