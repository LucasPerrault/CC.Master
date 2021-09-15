import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  FormGroup, NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { IDateRange } from '@cc/common/date';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';

import { defaultDateRangeConfiguration, IDateRangeConfiguration } from './date-range-configuration.interface';

@Component({
  selector: 'cc-date-range-select',
  templateUrl: './date-range-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateRangeSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: DateRangeSelectComponent,
    },
  ],
})
export class DateRangeSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public configuration: IDateRangeConfiguration = defaultDateRangeConfiguration;
  @Input() public datesClass?: string;
  @Input() public required = false;
  @Input() public isError = false;

  public onChange: (range: IDateRange) => void;
  public onTouch: () => void;

  public dateRangeSelected: FormGroup;

  private destroySubscription$: Subject<void> = new Subject<void>();

  constructor() {
    this.dateRangeSelected = new FormGroup({
      startDate: new FormControl(null),
      endDate: new FormControl(null),
    });
  }

  public ngOnInit(): void {
    this.dateRangeSelected.valueChanges
      .pipe(debounceTime(300), takeUntil(this.destroySubscription$))
      .subscribe((dateRange: IDateRange) => this.onChange(dateRange));
  }

  public ngOnDestroy(): void {
    this.destroySubscription$.next();
    this.destroySubscription$.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(rangeSelectionUpdated: IDateRange): void {
    if (rangeSelectionUpdated !== this.dateRangeSelected.value && rangeSelectionUpdated != null) {
      this.dateRangeSelected.patchValue(rangeSelectionUpdated);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.dateRangeSelected.invalid) {
      return { invalid: true };
    }
  }

  public get startDate(): Date {
    return this.dateRangeSelected.get('startDate').value;
  }

  public get endDate(): Date {
    return this.dateRangeSelected.get('endDate').value;
  }
}
