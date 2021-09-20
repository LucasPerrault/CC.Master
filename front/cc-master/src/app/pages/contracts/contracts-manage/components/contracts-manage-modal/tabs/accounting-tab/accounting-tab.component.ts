import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { AccountType, getAccount } from './enums/account-type.enum';
import { IAccountingEntry } from './models/accounting-entry.interface';
import { AccountingEntryListService } from './services/accounting-entry-list.service';

@Component({
  selector: 'cc-accounting-tab',
  templateUrl: './accounting-tab.component.html',
})
export class AccountingTabComponent implements OnInit {
  public get isLoading$(): Observable<boolean> {
    return combineLatest([
      this.paginatedLuccaEntries.state$.pipe(map(state => state === PaginatedListState.Update)),
      this.paginatedClientEntries.state$.pipe(map(state => state === PaginatedListState.Update)),
    ]).pipe(map(loadings => loadings.every(isLoading => !!isLoading)));
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  public luccaAccount = getAccount(AccountType.Lucca);
  public luccaBalance$: ReplaySubject<number> = new ReplaySubject<number>(1);
  public paginatedLuccaEntries: PaginatedList<IAccountingEntry>;

  public clientAccount = getAccount(AccountType.Client);
  public clientBalance$: ReplaySubject<number> = new ReplaySubject<number>(1);
  public paginatedClientEntries: PaginatedList<IAccountingEntry>;

  constructor(
    private activatedRoute: ActivatedRoute,
    private pagingService: PagingService,
    private accountingListService: AccountingEntryListService,
  ) { }

  public ngOnInit(): void {
    this.paginatedLuccaEntries = this.pagingService.paginate<IAccountingEntry>((httpParams) =>
      this.accountingListService.getAccountingEntries$(this.contractId, AccountType.Lucca, httpParams).pipe(
        map(response => ({ items: response.items, totalCount: response.count })),
      ),
    );

    this.paginatedLuccaEntries.items$
      .pipe(this.accountingListService.toBalance, take(1))
      .subscribe(balance => this.luccaBalance$.next(balance));

    this.paginatedClientEntries = this.pagingService.paginate<IAccountingEntry>((httpParams) =>
      this.accountingListService.getAccountingEntries$(this.contractId, AccountType.Client, httpParams).pipe(
        map(response => ({ items: response.items, totalCount: response.count })),
      ),
    );

    this.paginatedClientEntries.items$
      .pipe(this.accountingListService.toBalance, take(1))
      .subscribe(balance => this.clientBalance$.next(balance));

    this.init();
  }

  public updateLuccaEntriesFilters(isLettered: boolean): void {
    const params = this.accountingListService.toHttpParams(isLettered);
    return this.paginatedLuccaEntries.updateHttpParams(params);
  }

  public updateClientEntriesFilters(isLettered: boolean): void {
    const params = this.accountingListService.toHttpParams(isLettered);
    return this.paginatedClientEntries.updateHttpParams(params);
  }

  private init(): void {
    this.updateClientEntriesFilters(false);
    this.updateLuccaEntriesFilters(false);
  }
}
