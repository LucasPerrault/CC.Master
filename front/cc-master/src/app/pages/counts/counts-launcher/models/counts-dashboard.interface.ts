import { Observable, pipe, UnaryFunction } from 'rxjs';
import { map } from 'rxjs/operators';

export interface ICountsDashboard {
  countPeriod: Date;

  numberOfContracts: number;
  numberOfRealCounts: number;

  contractIdsWithoutCounts: number[];
  numberOfContractsWithoutCount: number;

  contractIdsWithDraftCount: number[];
  numberOfContractsWithDraftCount: number;

  contractIdsWithCountWithoutAccountingEntry: number[];
  numberOfContractsWithCountWithoutAccountingEntry: number;
}


export const toDashboard = (countPeriod: Date):
  UnaryFunction<Observable<[number, number, number[], number[], number[]]>, Observable<ICountsDashboard>> => pipe(
    map(([
      numberOfContracts,
      numberOfRealCounts,
      contractIdsWithoutCounts,
      contractIdsWithDraftCount,
      contractIdsWithCountWithoutAccountingEntry,
    ]) => ({
      countPeriod,
      numberOfContracts,
      numberOfRealCounts,
      contractIdsWithoutCounts,
      numberOfContractsWithoutCount: contractIdsWithoutCounts.length,
      contractIdsWithDraftCount,
      numberOfContractsWithDraftCount: contractIdsWithDraftCount.length,
      contractIdsWithCountWithoutAccountingEntry,
      numberOfContractsWithCountWithoutAccountingEntry: contractIdsWithCountWithoutAccountingEntry.length,
    })));
