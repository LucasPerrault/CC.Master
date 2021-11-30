import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiSortHelper, IHttpApiV3CollectionCount } from '@cc/common/queries';
import { Observable, pipe, UnaryFunction } from 'rxjs';
import { filter, map } from 'rxjs/operators';

import { AccountingEntryJournalCodes } from '../enums/accounting-entry-journal-code.enum';
import { IAccountingEntry } from '../models/accounting-entry.interface';
import { AccountingEntryDataService } from './accounting-entry-data.service';

@Injectable()
export class AccountingEntryListService {
  constructor(private accountingDataService: AccountingEntryDataService) {}

  public get toBalance(): UnaryFunction<Observable<IAccountingEntry[]>, Observable<number>> {
    return pipe(
      filter(entries => !!entries.length),
      map(entries => this.getBalance(entries)),
    );
  }

  public toHttpParams(isLettered: boolean): HttpParams {
    return isLettered ? new HttpParams() : new HttpParams().set('letter', 'null');
  }

  public getAccountingEntries$(
    contractId: number,
    accountNumber: number,
    httpParams: HttpParams,
  ): Observable<IHttpApiV3CollectionCount<IAccountingEntry>> {
    const params = httpParams
      .set('contractId', `${ contractId }`)
      .set('accountNumber', `like,${ accountNumber }`)
      .set('journalCode', `notequal,${ AccountingEntryJournalCodes.Draft }`)
      .set(ApiSortHelper.v3SortKey, `periodOn,${ ApiSortHelper.v3DscKey }`);

    return this.accountingDataService.getAccountingEntries$(params);
  }

  private getBalance(entries: IAccountingEntry[]): number {
    const unletteredEntries = entries.filter(e => !e.letter);
    const entriesCredited = unletteredEntries.filter(a => a.isCredit);
    const totalCredited = entriesCredited
      .reduce((t, entry) => t += entry.euroAmount, 0);

    const entriesDebited = unletteredEntries.filter(a => !a.isCredit);
    const totalDebited = entriesDebited
      .reduce((t, entry) => t += entry.euroAmount, 0);

    return totalCredited - totalDebited;
  }

}
