import { Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ILogsFilter } from '../../models/logs-filter.interface';

enum LogsFilterKey {
  Users = 'users',
  Environments = 'environments',
  Actions = 'actions',
  CreatedOn = 'createdOn',
  Domains = 'domains',
  IsAnonymized = 'isAnonymized'
}

@Component({
  selector: 'cc-logs-filter',
  templateUrl: './logs-filter.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => LogsFiltersComponent),
      multi: true,
    },
  ],
})
export class LogsFiltersComponent implements ControlValueAccessor, OnInit, OnDestroy {
  public onChange: (logsFilter: ILogsFilter) => void;
  public onTouch: () => void;

  public showMoreFilters = false;

  public filtersFormGroup: FormGroup;
  public filtersKey = LogsFilterKey;

  private readonly additionalFiltersKey = [LogsFilterKey.Domains, LogsFilterKey.IsAnonymized];

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.filtersFormGroup = new FormGroup({
      [LogsFilterKey.Users]:  new FormControl([]),
      [LogsFilterKey.Environments]:  new FormControl([]),
      [LogsFilterKey.Actions]:  new FormControl([]),
      [LogsFilterKey.Domains]:  new FormControl([]),
      [LogsFilterKey.IsAnonymized]:  new FormControl(null),
      [LogsFilterKey.CreatedOn]:  new FormControl({
        startDate: null,
        endDate: null,
      }),
    });
  }

  public ngOnInit(): void {
    this.filtersFormGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(filters => this.onChange(filters));
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

  public writeValue(logsFilter: ILogsFilter): void {
    if (!logsFilter) {
      return;
    }

    this.filtersFormGroup.setValue(logsFilter, { emitEvent: false });

    if (this.hasAdditionalFiltersSelected()) {
      this.toggleMoreFiltersDisplay();
    }
  }

  public toggleMoreFiltersDisplay(): void {
    this.showMoreFilters = !this.showMoreFilters;
  }

  public getAdditionalFiltersKeySelected(): LogsFilterKey[] {
    return this.additionalFiltersKey.filter(key => {
      if (key === LogsFilterKey.IsAnonymized) {
        return this.filtersFormGroup.get(key).value !== null;
      }

      return !!this.filtersFormGroup.get(key).value.length;
    });
  }

  public hasAdditionalFiltersSelected(): boolean {
    return !!this.getAdditionalFiltersKeySelected().length;
  }
}
