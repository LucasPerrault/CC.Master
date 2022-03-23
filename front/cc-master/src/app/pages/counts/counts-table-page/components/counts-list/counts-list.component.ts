import { CurrencyPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { NavigationPath } from '@cc/common/navigation';
import { PaginatedListState } from '@cc/common/paging';
import { ISortParams, SortOrder, SortService } from '@cc/common/sort';
import { DistributorUtilsService } from '@cc/domain/billing/distributors/services/distributor-utils.service';
import { CurrencyName, ICurrency } from '@cc/domain/billing/offers';

import { CountsSortParamKey } from '../../enums/count-sort-param-key.enum';
import { ICountsSummary } from '../../models/counts-summary.interface';
import { IDetailedCount } from '../../models/detailed-count.interface';
import { CountAdditionalColumn, countAdditionalColumns } from '../count-additional-column-select/count-additional-column.enum';

@Component({
  selector: 'cc-counts-list',
  templateUrl: './counts-list.component.html',
  styleUrls: ['./counts-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CountsListComponent implements OnInit {
  @Input() public counts: IDetailedCount[];
  @Input() public summary: ICountsSummary;
  @Input() public sortParams: ISortParams;
  @Input() public state: PaginatedListState;
  @Input() public columnsSelected: CountAdditionalColumn[];
  @Output() public sort: EventEmitter<ISortParams> = new EventEmitter<ISortParams>();

  @ViewChild('table') tableElementRef: ElementRef;

  public get columnsNumber(): number {
    return this.tableElementRef?.nativeElement?.rows[0].cells?.length ?? 0;
  }


  public get isEmpty(): boolean {
    return this.isIdle && !this.counts.length;
  }

  public get isIdle(): boolean {
    return this.state === PaginatedListState.Idle || this.state === PaginatedListState.Error;
  }

  public get isLoadingMore(): boolean {
    return this.state === PaginatedListState.LoadMore;
  }

  public sortParamKey = CountsSortParamKey;
  public sortOrder = SortOrder;

  public column = CountAdditionalColumn;

  constructor(
    private sortService: SortService,
    private currencyPipe: CurrencyPipe,
    private translatePipe: TranslatePipe,
  ) { }

  public ngOnInit(): void {
  }

  public trackBy(index: number, count: IDetailedCount): number {
    return count.id;
  }

  public sortBy(field: string, order: SortOrder = SortOrder.Asc): void {
    this.sortParams = this.sortService.updateSortParam(field, order, this.sortParams);
    this.sort.emit(this.sortParams);
  }

  public getSortOrderClass(field: string): string {
    return this.sortService.getSortOrderClass(field, this.sortParams);
  }

  public redirectToContract(contractId: number): void {
    window.open(`${ NavigationPath.Contracts }/${ NavigationPath.ContractsManage }/${ contractId }`);
  }

  public getUnitPrice(unitPrice: number, currency: ICurrency): string {
    const currencyCode = currency?.name ?? CurrencyName.EUR;
    const originalAmount = this.currencyPipe.transform(unitPrice, currencyCode,'symbol', '1.2');

    return currencyCode !== CurrencyName.EUR ? `${ originalAmount } (${ this.toEuro(unitPrice) })` : originalAmount;
  }

  public toEuro(value: number): string {
    return this.currencyPipe.transform(value, CurrencyName.EUR,'symbol', '1.2');
  }

  public getAdditionalColumnName(id: CountAdditionalColumn): string {
    return this.translatePipe.transform(countAdditionalColumns.find(c => c.id === id).name);
  }

  public isHidden(columns: CountAdditionalColumn[]): boolean {
    return !this.columnsSelected?.length || !this.columnsSelected?.some(c => columns.includes(c));
  }

  public getActiveColumns(columns: CountAdditionalColumn[]): number {
    return columns.filter(c => !this.isHidden([c]))?.length;
  }

  public getOnlyDistributorName(codeAndName: string) {
    return DistributorUtilsService.getOnlyName(codeAndName);
  }
}
