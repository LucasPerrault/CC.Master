import { Component, EventEmitter, HostBinding, Input, OnInit, Output, ViewChild} from '@angular/core';
import {CdkVirtualScrollViewport} from '@angular/cdk/scrolling';
import {ApiV3Order, IApiV3SortParams} from '../../queries';
import {EnvironmentLogMessageType} from '../../enums';
import {IEnvironment, IEnvironmentLog} from '../../models';

@Component({
  selector: 'cc-logs-list',
  templateUrl: './logs-list.component.html',
  styleUrls: ['./logs-list.component.scss'],
})
export class LogsListComponent implements OnInit {
  @ViewChild(CdkVirtualScrollViewport, { static: true, read: CdkVirtualScrollViewport })
  private _scrollViewport: CdkVirtualScrollViewport;

  @Input() public logs: IEnvironmentLog[];
  @Input() public defaultSortParams: IApiV3SortParams;
  @Input() public isRefreshedDataLoading: boolean;
  @Input() public isShownMoreDataLoading: boolean;
  @Output() public sortByParams: EventEmitter<IApiV3SortParams> = new EventEmitter<IApiV3SortParams>();
  @Output() public showMoreData: EventEmitter<void> = new EventEmitter<void>();

  public readonly rowHeightFixed = 42;
  @HostBinding("style.--row-height-in-px")
  public readonly rowHeightFixedInPixel = `${this.rowHeightFixed}px`;

  private _sortParams: IApiV3SortParams;

  public ngOnInit(): void {
    this._sortParams = this.defaultSortParams;
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
    const rowNumberBeforeBottomToShowMore = 15;
    const rowsHeightStepToShowMore = this.rowHeightFixed * rowNumberBeforeBottomToShowMore;
    if (this._scrollViewport.measureScrollOffset("bottom") <= rowsHeightStepToShowMore) {
      this.showMoreData.emit();
    }
  }

  public activeAscOrDescIcon(field: string, order: string): boolean {
    if (!this._sortParams) {
      return false;
    }
    return this._sortParams.field === field && this._sortParams.order === order;
  }

  public sortBy(field: string, order: ApiV3Order = 'asc'): void {
    this._sortParams = {
      field: field,
      order: this.getOrderToSort(field, order)
    };

    this.sortByParams.emit(this._sortParams);
  }

  private getOrderToSort(fieldToSort: string, orderToSort: ApiV3Order): ApiV3Order {
    if (fieldToSort === this._sortParams.field) {
      return this._sortParams.order === 'asc' ? 'desc' : 'asc';
    }

    return orderToSort;
  }
}
