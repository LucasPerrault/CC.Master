import { HttpParams } from '@angular/common/http';
import { Component } from '@angular/core';
import { PaginatedList, PaginatedListState } from '@cc/common/paging';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IEnvironment } from './models/environment.interface';
import { EnvironmentListService } from './services/environment-list.service';
import { FormControl } from '@angular/forms';

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

  constructor(private environmentsService: EnvironmentListService) {
    this.paginatedEnvironments = this.environmentsService.getPaginatedEnvironments$();
  }

  public nextPage(): void {
    this.paginatedEnvironments.nextPage();
  }

  public updateHttpParams(params: HttpParams): void {
    this.paginatedEnvironments.updateHttpParams(params);
  }
}
