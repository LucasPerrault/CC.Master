import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ApiStandard } from '@cc/common/queries';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { map, skip, takeUntil } from 'rxjs/operators';

import { IEstablishment } from '../common/models/establishment.interface';
import { EstablishmentsDataService } from './services/establishments-data.service';

@Component({
  selector: 'cc-establishments',
  templateUrl: './establishments.component.html',
  styleUrls: ['./establishments.component.scss'],
})
export class EstablishmentsComponent implements OnInit, OnDestroy {
  public get establishments$(): Observable<IEstablishment[]> { return this.paginatedEts.items$; }
  public get totalCount$(): Observable<number> { return this.paginatedEts.totalCount$; }
  public isLoading$ = new ReplaySubject<boolean>(1);

  private paginatedEts: PaginatedList<IEstablishment>;
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private pagingService: PagingService,
    private dataService: EstablishmentsDataService,
  ) {
    this.paginatedEts = this.pagingService.paginate<IEstablishment>(
      (httpParams) => this.getEstablishments$(httpParams),
      { page: defaultPagingParams.page, limit: 50 },
    );
  }

  public ngOnInit(): void {
    this.paginatedEts.state$
      .pipe(takeUntil(this.destroy$), skip(1), map(state => state === PaginatedListState.Update))
      .subscribe(isLoading => this.isLoading$.next(isLoading));

    this.paginatedEts.updateHttpParams(new HttpParams());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedEts.nextPage();
  }

  private refresh(): void {
    this.paginatedEts.updateHttpParams(new HttpParams());
  }

  private getEstablishments$(httpParams: HttpParams): Observable<IPaginatedResult<IEstablishment>> {
    return this.dataService.getEstablishments$(httpParams)
      .pipe(map(response => ({ items: response.items, totalCount: response.count })));
  }
}
