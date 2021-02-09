import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { Component, EventEmitter, HostBinding, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { EnvironmentLogMessageType, IEnvironment, IEnvironmentLog } from '@cc/domain/environments';

@Component({
  selector: 'cc-logs-list',
  templateUrl: './logs-list.component.html',
  styleUrls: ['./logs-list.component.scss'],
})
export class LogsListComponent implements OnInit {
  @ViewChild(CdkVirtualScrollViewport, { static: true, read: CdkVirtualScrollViewport })
  public scrollViewport: CdkVirtualScrollViewport;

  @Input() public logs: IEnvironmentLog[];
  @Input() public defaultSortParams: ISortParams;
  @Input() public isUpdateData: boolean;
  @Input() public isLoadMore: boolean;
  @Output() public updateSort: EventEmitter<ISortParams> = new EventEmitter<ISortParams>();
  @Output() public showMore: EventEmitter<void> = new EventEmitter<void>();

  @HostBinding('style.--row-height-in-px')
  public readonly rowHeightFixedInPixel = `42px`;
  public readonly rowHeightFixed = 42;
  private rowNumberBeforeBottomToShowMore = 15;

  private sortParams: ISortParams;

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

  public scroll(): void {
    const rowsHeightStepToShowMore = this.rowHeightFixed * this.rowNumberBeforeBottomToShowMore;
    if (this.scrollViewport.measureScrollOffset('bottom') <= rowsHeightStepToShowMore) {
      this.showMore.emit();
    }
  }

  public activeAscOrDescIcon(field: string, order: string): boolean {
    if (!this.sortParams) {
      return false;
    }
    return this.sortParams.field === field && this.sortParams.order === order;
  }

  public sortBy(field: string, order: SortOrder = 'asc'): void {
    this.sortParams = {
      field,
      order: this.getOrderToSort(field, order),
    };

    this.updateSort.emit(this.sortParams);
  }

  private getOrderToSort(fieldToSort: string, orderToSort: SortOrder): SortOrder {
    if (fieldToSort === this.sortParams.field) {
      return this.sortParams.order === 'asc' ? 'desc' : 'asc';
    }

    return orderToSort;
  }
}
