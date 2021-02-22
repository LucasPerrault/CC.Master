import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PaginatedListState } from '@cc/common/paging';
import { ISortParams, SortOrder, SortService } from '@cc/common/sort';
import { EnvironmentLogMessageType, IEnvironment, IEnvironmentLog } from '@cc/domain/environments';

@Component({
  selector: 'cc-logs-list',
  templateUrl: './logs-list.component.html',
  styleUrls: ['./logs-list.component.scss'],
})
export class LogsListComponent implements OnInit {
  @Input() public logs: IEnvironmentLog[];
  @Input() public defaultSortParams: ISortParams;
  @Output() public updateSort: EventEmitter<ISortParams> = new EventEmitter<ISortParams>();
  @Output() public showMore: EventEmitter<void> = new EventEmitter<void>();
  @Input() public state: PaginatedListState;

  public sortOrder = SortOrder;
  private sortParams: ISortParams;

  constructor(private sortService: SortService) {
  }

  public ngOnInit(): void {
    this.sortParams = this.defaultSortParams;
  }

  public getInstanceName(environment: IEnvironment): string {
    return !!environment ? `${environment.subDomain}.${environment.domainName}` : '';
  }

  public getNonAnonymizedActionExplanation(log: IEnvironmentLog): string {
    if (log.isAnonymizedData) {
      return '';
    }
    const justificationLogMessage = log.messages.find(m => m.type === EnvironmentLogMessageType.Explanation);
    return !!justificationLogMessage ? justificationLogMessage.message : '';
  }

  public showMoreData(): void {
    this.showMore.emit();
  }

  public activeAscOrDescIcon(field: string, order: SortOrder): boolean {
    return this.sortService.isSorted(order, field, [this.sortParams]);
  }

  public sortBy(field: string, order: SortOrder = SortOrder.Asc): void {
    this.sortParams = this.sortService.updateSortParam(field, order, [this.sortParams])[0];
    this.updateSort.emit(this.sortParams);
  }

  public get isLoading(): boolean {
    return this.isUpdateData || this.isLoadMore;
  }

  public get isIdle(): boolean {
    return this.state === PaginatedListState.Idle || this.state === PaginatedListState.Error;
  }

  public get isUpdateData(): boolean {
    return this.state === PaginatedListState.UpdateFilter || this.state === PaginatedListState.UpdateSort;
  }

  public get isLoadMore(): boolean {
    return this.state === PaginatedListState.LoadMore;
  }

}
