import { DatePipe } from '@angular/common';
import { Component, ElementRef, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { PaginatedListState } from '@cc/common/paging';
import { ISortParams, SortOrder, SortService } from '@cc/common/sort';
import { IEstablishment } from '@cc/domain/billing/establishments';
import { Subject } from 'rxjs';

import { ContractAdditionalColumn, contractAdditionalColumns } from '../../constants/contract-additional-column.enum';
import { ContractEstablishmentHealth, contractEstablishmentsHealth } from '../../constants/contract-establishment-health.enum';
import { ContractSortParamKey } from '../../constants/contract-sort-param-key.enum';
import { IContractEstablishmentHealth } from '../../models/contract-establishment-health.interface';
import { IContractListEntry } from '../../models/contract-list-entry.interface';

@Component({
  selector: 'cc-contracts-manage-list',
  templateUrl: './contracts-manage-list.component.html',
  styleUrls: ['./contracts-manage-list.component.scss'],
})
export class ContractsManageListComponent {
  @Input() public contracts: IContractListEntry[];
  @Input() public state: PaginatedListState;
  @Input() public sortParams: ISortParams;
  @Input() public columnsSelected: ContractAdditionalColumn[];
  @Output() public updateSort: EventEmitter<ISortParams> = new EventEmitter<ISortParams>();

  public contractHiddenColumn = ContractAdditionalColumn;
  public sortParamKey = ContractSortParamKey;
  public sortOrder = SortOrder;

  public hasHorizontalScroll$: Subject<boolean> = new Subject<boolean>();

  public get isEmpty(): boolean {
    return this.isIdle && !this.contracts.length;
  }

  public get isIdle(): boolean {
    return this.state === PaginatedListState.Idle || this.state === PaginatedListState.Error;
  }

  public get isLoadingMore(): boolean {
    return this.state === PaginatedListState.LoadMore;
  }

  constructor(
    private translatePipe: TranslatePipe,
    private datePipe: DatePipe,
    private sortService: SortService,
    private elementRef: ElementRef,
  ) {}

  @HostListener('scroll') public scroll(): void {
    this.hasHorizontalScroll$.next(this.isScrollHorizontalEnabled(this.elementRef));
  }

  public sortBy(field: string, order: SortOrder = SortOrder.Asc): void {
    this.sortParams = this.sortService.updateSortParam(field, order, this.sortParams);
    this.updateSort.emit(this.sortParams);
  }

  public getSortOrderClass(field: string): string {
    return this.sortService.getSortOrderClass(field, this.sortParams);
  }

  public getEstablishmentNames(establishments: IEstablishment[]): string {
    return establishments.map(e => e.name).join(', ');
  }

  public getOnlyDistributorName(codeAndName: string): string {
    const codeAndNameSeparator = '-';
    const separatorIndex = codeAndName.indexOf(codeAndNameSeparator);
    return codeAndName.slice(separatorIndex + 1);
  }

  public getEstablishmentHealthName(contract: IContractListEntry): string {
    const establishmentHealth = this.getEstablishmentHealth(contract);
    if (establishmentHealth.id === ContractEstablishmentHealth.Error) {
      return this.translatePipe.transform(establishmentHealth.name, { count: contract.leErrorNumber });
    }

    return this.translatePipe.transform(establishmentHealth.name);
  }

  public getEstablishmentHealth(contract: IContractListEntry): IContractEstablishmentHealth {
    if (contract.leErrorNumber !== 0) {
      return this.getEstablishmentHealthById(ContractEstablishmentHealth.Error);
    }

    if (!contract.environmentId) {
      return this.getEstablishmentHealthById(ContractEstablishmentHealth.NoEnvironment);
    }

    if (!contract.startOn) {
      return this.getEstablishmentHealthById(ContractEstablishmentHealth.NoEstablishment);
    }

    return this.getEstablishmentHealthById(ContractEstablishmentHealth.Ok);
  }

  public getContractStatus(startAtToString: string, endAtToString: string): string {
    if (!endAtToString && !startAtToString) {
      return '';
    }

    const today = new Date();
    if (!!endAtToString) {
      const endAt = new Date(endAtToString);
      const isFinished = endAt <= today;

      const translationKey = isFinished ? 'front_contractPage_column_status_isFinished' : 'front_contractPage_column_status_willFinish';

      return this.translatePipe.transform(translationKey, {
        endAt: this.datePipe.transform(endAt, 'shortDate'),
      });
    }

    const startAt = new Date(startAtToString);
    const isStarted = startAt <= today;

    const translationKey = isStarted ? 'front_contractPage_column_status_isStarted' : 'front_contractPage_column_status_willStart';

    return this.translatePipe.transform(translationKey, {
      startAt: this.datePipe.transform(startAt, 'shortDate'),
    });
  }

  public getHiddenColumnName(id: ContractAdditionalColumn): string {
    return this.translatePipe.transform(contractAdditionalColumns.find(c => c.id === id).name);
  }

  public isHidden(id: ContractAdditionalColumn): string {
    return !!this.columnsSelected.find(c => c === id) ? '' : 'mod-hidden';
  }

  public getContractVintage(contract: IContractListEntry): string {
    const year = this.datePipe.transform(contract.theoricalStartOn, 'yy');
    return `${ contract.product.code }${ year }`;
  }

  private getEstablishmentHealthById(id: ContractEstablishmentHealth): IContractEstablishmentHealth {
    return contractEstablishmentsHealth.find(e => e.id === id);
  }

  private isScrollHorizontalEnabled(elementRef: ElementRef): boolean {
    return elementRef.nativeElement.scrollWidth > elementRef.nativeElement.clientWidth;
  }
}
