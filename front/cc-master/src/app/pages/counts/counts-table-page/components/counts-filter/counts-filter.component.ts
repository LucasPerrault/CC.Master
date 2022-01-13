import { ChangeDetectionStrategy, Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import {
  ControlValueAccessor, FormControl,
  FormGroup,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';
import { SelectDisplayMode } from '@cc/common/forms';
import { EndDateGranularityPolicy, IDateRangeConfiguration } from '@cc/common/forms/select/date-range-select';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { startOfMonth, subMonths } from 'date-fns';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CountsFilterFormKey, ICountsFilterForm } from '../../models/counts-filter-form.interface';

@Component({
  selector: 'cc-counts-filter',
  templateUrl: './counts-filter.component.html',
  styleUrls: ['./counts-filter.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CountsFilterComponent),
      multi: true,
    },
  ],
})
export class CountsFilterComponent implements OnInit, OnDestroy, ControlValueAccessor {

  public formGroup: FormGroup;
  public formKey = CountsFilterFormKey;
  public selectMode = SelectDisplayMode;

  public showAdditionalFilters = false;
  private readonly additionalFiltersKey = [
    CountsFilterFormKey.Distributors,
    CountsFilterFormKey.Offers,
    CountsFilterFormKey.EnvironmentGroups,
    CountsFilterFormKey.Products,
    CountsFilterFormKey.Clients,
  ];

  private destroy$: Subject<void> = new Subject();

  constructor() {
    this.formGroup = new FormGroup({
      [CountsFilterFormKey.CountPeriod]: new FormControl({
        startDate: null,
        endDate: null,
      }),
      [CountsFilterFormKey.Clients]: new FormControl([]),
      [CountsFilterFormKey.Distributors]: new FormControl([]),
      [CountsFilterFormKey.Offers]: new FormControl([]),
      [CountsFilterFormKey.EnvironmentGroups]: new FormControl([]),
      [CountsFilterFormKey.Products]: new FormControl([]),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: ICountsFilterForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: ICountsFilterForm): void {
    if (!!form && form !== this.formGroup.value) {
      this.formGroup.patchValue(form);
    }
  }

  public toggleAdditionalFiltersDisplay(): void {
    this.showAdditionalFilters = !this.showAdditionalFilters;
  }

  private isSelected(formGroup: FormGroup, key: CountsFilterFormKey): boolean {
    const formControl = formGroup.get(key);
    return Array.isArray(formControl.value) ? !!formControl.value.length : !!formControl.value;
  }

  public get selectedAdditionalFiltersCount(): number {
    return this.additionalFiltersKey.filter(key => this.isSelected(this.formGroup, key)).length;
  }

  public get dateRangeConfiguration(): IDateRangeConfiguration {
    const monthWithFirstCounts = startOfMonth(new Date('2002-03-01'));
    const monthWithLastCounts = startOfMonth(subMonths(Date.now(), 1));

    return {
      granularity: ELuDateGranularity.month,
      periodCoverStrategy: EndDateGranularityPolicy.Beginning,
      startDateConfiguration: {
        min: monthWithFirstCounts,
        max: monthWithLastCounts,
        class: 'mod-inline palette-grey mod-outlined',
      },
      endDateConfiguration: {
        min: monthWithFirstCounts,
        max: monthWithLastCounts,
        class: 'mod-inline palette-grey mod-outlined',
      },
    };
  }


}
