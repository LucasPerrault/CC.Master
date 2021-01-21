import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { Component, EventEmitter, HostBinding, Input, OnInit, Output, ViewChild } from '@angular/core';

import { ApiV3Order, IApiV3SortParams } from '../../../../common/queries';
import { EnvironmentLogMessageType } from '../../enums';
import { IEnvironment, IEnvironmentLog } from '../../models';

@Component({
  selector: 'cc-logs-list',
  templateUrl: './logs-list.component.html',
  styleUrls: ['./logs-list.component.scss'],
})
export class LogsListComponent implements OnInit {
  @ViewChild(CdkVirtualScrollViewport, { static: true, read: CdkVirtualScrollViewport })
  public scrollViewport: CdkVirtualScrollViewport;

  @Input() public logs: IEnvironmentLog[];
  @Input() public defaultSortParams: IApiV3SortParams;
  @Input() public isRefreshedDataLoading: boolean;
  @Input() public isShownMoreDataLoading: boolean;
  @Output() public sortByParams: EventEmitter<IApiV3SortParams> = new EventEmitter<IApiV3SortParams>();
  @Output() public showMoreData: EventEmitter<void> = new EventEmitter<void>();

  @HostBinding('style.--row-height-in-px')
  public readonly rowHeightFixedInPixel = `42px`;
  public readonly rowHeightFixed = 42;
  private rowNumberBeforeBottomToShowMore = 15;

  private sortParams: IApiV3SortParams;

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
      this.showMoreData.emit();
    }
  }

  public activeAscOrDescIcon(field: string, order: string): boolean {
    if (!this.sortParams) {
      return false;
    }
    return this.sortParams.field === field && this.sortParams.order === order;
  }

  public sortBy(field: string, order: ApiV3Order = 'asc'): void {
    this.sortParams = {
      field,
      order: this.getOrderToSort(field, order),
    };

    this.sortByParams.emit(this.sortParams);
  }

  private getOrderToSort(fieldToSort: string, orderToSort: ApiV3Order): ApiV3Order {
    if (fieldToSort === this.sortParams.field) {
      return this.sortParams.order === 'asc' ? 'desc' : 'asc';
    }

    return orderToSort;
  }
}
