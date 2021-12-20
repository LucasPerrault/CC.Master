import { Injectable } from '@angular/core';
import { ICount } from '@cc/domain/billing/counts';
import { combineLatest, Observable, pipe, ReplaySubject, UnaryFunction } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { IContractEntry, IValidationContext, ValidationContextDataService } from './validation-context-store.data';

@Injectable()
export class ValidationContextStoreService {
  public get context$(): Observable<IValidationContext> {
    const context$ = [this.activeEtsNumber.asObservable(), this.realCounts.asObservable(), this.contractEntries.asObservable()];
    return combineLatest(context$).pipe(this.mapToValidationContext);
  }

  public get activeEtsNumber$(): Observable<number> {
    return this.activeEtsNumber.asObservable();
  }

  public get realCounts$(): Observable<ICount[]> {
    return this.realCounts.asObservable();
  }

  public get isLoading$(): Observable<boolean> {
    return this.isLoading.asObservable();
  }

  private activeEtsNumber: ReplaySubject<number> = new ReplaySubject<number>(1);
  private realCounts: ReplaySubject<ICount[]> = new ReplaySubject<ICount[]>(1);
  private contractEntries: ReplaySubject<IContractEntry[]> = new ReplaySubject<IContractEntry[]>(1);

  private isLoading: ReplaySubject<boolean> = new ReplaySubject<boolean>();

  constructor(private dataService: ValidationContextDataService) {}

  public init(contractId: number): void {
    this.refreshAll(contractId);
  }

  public refreshAll(contractId: number): void {
    this.isLoading.next(true);

    combineLatest([
      this.dataService.getActiveEstablishmentNumber$(contractId),
      this.dataService.getRealCounts$(contractId),
      this.dataService.getContractEntries$(contractId),
    ])
      .pipe(take(1), this.mapToValidationContext, finalize(() => this.isLoading.next(false)))
      .subscribe(context => {
        this.activeEtsNumber.next(context.activeEstablishmentNumber);
        this.contractEntries.next(context.contractEntries);
        this.realCounts.next(context.realCounts);
      });
  }

  private get mapToValidationContext(): UnaryFunction<Observable<[number, ICount[], IContractEntry[]]>, Observable<IValidationContext>> {
    return pipe(
      map(([activeEstablishmentNumber, realCounts, contractEntries]) => ({
        activeEstablishmentNumber,
        contractEntries,
        realCounts,
      })),
    );
  }
}
