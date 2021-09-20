import { HttpParams } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { DownloadService, DownloadState } from '@cc/common/download';
import { IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { BehaviorSubject, combineLatest, Observable, Subject, timer } from 'rxjs';
import { debounceTime, map, shareReplay, startWith, take, takeUntil } from 'rxjs/operators';

import { ContractAdditionalColumn, IContractAdditionalColumn } from './constants/contract-additional-column.enum';
import { ContractSortParamKey } from './constants/contract-sort-param-key.enum';
import { IContractListEntry } from './models/contract-list-entry.interface';
import { IContractsFilter } from './models/contracts-filter.interface';
import { ContractsApiMappingService } from './services/contracts-api-mapping.service';
import { ContractsFilterRoutingService } from './services/contracts-filter-routing.service';
import { ContractsListService } from './services/contracts-list.service';
import { ContractsRoutingService } from './services/contracts-routing.service';

@Component({
  selector: 'cc-contracts-manage',
  templateUrl: './contracts-manage.component.html',
})
export class ContractsManageComponent implements OnInit, OnDestroy {
  public get contracts$(): Observable<IContractListEntry[]> {
    return this.paginatedContracts.items$;
  }

  public get contractsCount$(): Observable<number> {
    return this.paginatedContracts.totalCount$;
  }

  public get contractsState$(): Observable<PaginatedListState> {
    return this.paginatedContracts.state$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedContracts.state$.pipe(
      map(state => state === PaginatedListState.Update),
    );
  }

  public get downloadButtonStatus(): string {
    return this.getButtonStatus(this.downloadState$.value);
  }

  public filters: FormControl = new FormControl(null);
  public sortParams$: BehaviorSubject<ISortParams> = new BehaviorSubject<ISortParams>({
    field: ContractSortParamKey.Client,
    order: SortOrder.Asc,
  });

  public columnsSelectedFormControl: FormControl = new FormControl();
  public columnsSelected$: Observable<ContractAdditionalColumn[]>;

  private paginatedContracts: PaginatedList<IContractListEntry>;
  private downloadState$: BehaviorSubject<DownloadState> = new BehaviorSubject<DownloadState>(DownloadState.Idle);
  private destroy$: Subject<void> = new Subject<void>();

  constructor(
    private contractsListService: ContractsListService,
    private contractsFilterRoutingService: ContractsFilterRoutingService,
    private contractsRoutingService: ContractsRoutingService,
    private contractsApiMappingService: ContractsApiMappingService,
    private pagingService: PagingService,
    private downloadService: DownloadService,
  ) {}

  ngOnInit(): void {
    combineLatest([this.sortParams$, this.filters.valueChanges])
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(300),
        map(([sort, filters]) => ({ sort, filters })),
        map(attributes => this.contractsApiMappingService.toHttpParams(attributes)),
      )
      .subscribe(httpParams => this.paginatedContracts.updateHttpParams(httpParams));

    this.columnsSelected$ = this.columnsSelectedFormControl.valueChanges.pipe(
      map((columns: IContractAdditionalColumn[]) => !!columns ? columns.map(c => c.id) : []),
      startWith([]),
      shareReplay(1),
    );

    combineLatest([this.filters.valueChanges, this.columnsSelected$])
      .pipe(takeUntil(this.destroy$))
      .subscribe(async ([filters, columns]) => await this.updateRoutingParamsAsync(filters, columns));

    const routingParams = this.contractsRoutingService.getContractsRoutingParams();
    this.contractsFilterRoutingService.toContractsFilter$(routingParams)
      .pipe(take(1))
      .subscribe(filters => this.filters.setValue(filters));

    const routingColumnsSelected = this.contractsFilterRoutingService.toContractColumnsSelected(routingParams);
    this.columnsSelectedFormControl.setValue(routingColumnsSelected);

    this.paginatedContracts = this.pagingService.paginate<IContractListEntry>(
      (httpParams) => this.getPaginatedContracts$(httpParams),
    );

    this.contractsListService.onRefresh$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.refreshPaginatedContracts());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public nextPage(): void {
    this.paginatedContracts.nextPage();
  }

  public updateSort(sortParams: ISortParams) {
    this.sortParams$.next(sortParams);
  }

  public export(): void {
    this.downloadState$.next(DownloadState.Load);

    const httpParams = this.contractsApiMappingService.toHttpParams({
      filters: this.filters.value,
      sort: this.sortParams$.value,
    });

    const url = this.contractsListService.getExportContractsUrl(httpParams);

    this.downloadService.download$(url, httpParams)
      .pipe(takeUntil(this.destroy$))
      .subscribe(
        () => this.downloadState$.next(DownloadState.Success),
        err => this.downloadState$.next(DownloadState.Error),
        () => this.idleDownloadStateTimer(1000),
      );
  }

  public async updateRoutingParamsAsync(filters: IContractsFilter, columns: ContractAdditionalColumn[]): Promise<void> {
    const routingParams = this.contractsFilterRoutingService.toContractsRoutingParams(filters, columns);
    await this.contractsRoutingService.updateRouterAsync(routingParams);
  }

  private getPaginatedContracts$(httpParams: HttpParams): Observable<IPaginatedResult<IContractListEntry>> {
    return this.contractsListService.getContractsList$(httpParams).pipe(
      map(response => ({ items: response.items, totalCount: response.count })),
    );
  }

  private refreshPaginatedContracts(): void {
    const activatedHttpParams = this.contractsApiMappingService.toHttpParams({
      sort: this.sortParams$.value,
      filters: this.filters.value,
    });
    this.paginatedContracts.updateHttpParams(activatedHttpParams);
  }

  private getButtonStatus(state: DownloadState): string {
    switch (state) {
      case DownloadState.Success:
        return 'is-success';
      case DownloadState.Error:
        return 'is-error';
      case DownloadState.Load:
        return 'is-loading';
      case DownloadState.Idle:
      default:
        return '';
    }
  }

  private idleDownloadStateTimer(dueTime: number): void {
    timer(dueTime)
      .pipe(take(1))
      .subscribe(() => this.downloadState$.next(DownloadState.Idle));
  }
}
