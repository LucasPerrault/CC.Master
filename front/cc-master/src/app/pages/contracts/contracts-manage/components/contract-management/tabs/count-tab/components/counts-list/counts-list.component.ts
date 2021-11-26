import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { CountCode } from '@cc/domain/billing/counts';
import { BehaviorSubject, Subject } from 'rxjs';
import { skip, takeUntil } from 'rxjs/operators';

import { BillingStrategy } from '../../constants/billing-strategy.enum';
import { ICountContract } from '../../models/count-contract.interface';
import { ICountListEntry } from '../../models/count-list-entry.interface';
import { IDetailedCount } from '../../models/detailed-count.interface';
import { CountContractsRestrictionsService } from '../../services/count-contracts-restrictions.service';

@Component({
  selector: 'cc-counts-list',
  templateUrl: './counts-list.component.html',
  styleUrls: ['./counts-list.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      multi: true,
      useExisting: CountsListComponent,
    },
  ],
})
export class CountsListComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Output() charge: EventEmitter<Date> = new EventEmitter<Date>();
  @Input() public contract: ICountContract;
  @Input() public showDraftCounts: boolean;
  @Input() public set countListEntries(entries: ICountListEntry[]) {
    this.entries = entries;
    this.setAreCountsNumberEqual(entries);
  }

  public onChange: (d: IDetailedCount[]) => void;
  public onTouch: () => void;

  public entries: ICountListEntry[] = [];

  public areAccountingNumberEqual;

  public countsSelected$: BehaviorSubject<IDetailedCount[]>
    = new BehaviorSubject<IDetailedCount[]>([]);
  private destroy$: Subject<void> = new Subject<void>();

  constructor(private restrictionsService: CountContractsRestrictionsService, private translatePipe: TranslatePipe) { }

  public ngOnInit(): void {
    this.countsSelected$
      .pipe(takeUntil(this.destroy$), skip(1))
      .subscribe(countsSelected => this.onChange(countsSelected));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(counts: IDetailedCount[]): void {
    if (!!counts) {
      this.countsSelected$.next(counts);
    }
  }

  public isCountDraft(entry: ICountListEntry): boolean {
    return entry.count?.code === CountCode.Draft;
  }

  public isMinimalBillingStrategy(billingStrategy: BillingStrategy): boolean {
    return billingStrategy === BillingStrategy.MinimalBilling;
  }

  public isAllSelected(entries: ICountListEntry[]): boolean {
    const allCountsSelectable = this.getAllCountsSelectable(entries);
    return allCountsSelectable.length === this.countsSelected$.value.length;
  }

  public selectAll(entries: ICountListEntry[]): void {
    if (this.isAllSelected(entries)) {
      this.countsSelected$.next([]);
      return;
    }

    this.countsSelected$.next(this.getAllCountsSelectable(entries));
  }

  public isSelected(count: IDetailedCount): boolean {
    return !!this.countsSelected$.value.find(c => c.id === count.id);
  }

  public select(count: IDetailedCount): void {
    const isAlreadySelected = !!this.countsSelected$.value.find(c => c.id === count.id);
    const countsSelected = isAlreadySelected
      ? this.countsSelected$.value.filter(c => c.id !== count.id)
      : [count, ...this.countsSelected$.value];

    this.countsSelected$.next(countsSelected);
  }

  public chargeContract(missingCountPeriod: Date): void {
    this.charge.emit(missingCountPeriod);
  }

  public canChargeContract(): boolean {
    return this.restrictionsService.canChargeCount();
  }

  public canDeleteAtLeastOneCount(entries: ICountListEntry[]): boolean {
    return this.restrictionsService.canDeleteAtLeastOneCount(entries);
  }

  public canDeleteCount(entry: ICountListEntry): boolean {
    return this.restrictionsService.canDeleteCount(entry);
  }

  public getDeleteRightsInfoTooltip(entry: ICountListEntry): string {
    if (this.isCountDraft(entry)) {
      return this.translatePipe.transform('contracts_count_cannotDelete_draftCount_info');
    }

    if (!!entry.count && !this.canDeleteCount(entry)) {
      return this.translatePipe.transform('contracts_count_cannotDelete_realCount_info');
    }

    return '';
  }

  private setAreCountsNumberEqual(entries: ICountListEntry[]): void {
    const counts = entries.filter(entry => !!entry.count).map(entry => entry.count) ?? [];
    this.areAccountingNumberEqual = counts.every(count => count.accountingNumber === count.number);
  }

  private getAllCountsSelectable(entries: ICountListEntry[]): IDetailedCount[] {
    return this.restrictionsService.getAllCountEntriesSelectable(entries)
      .map(entry => entry.count);
  }
}
