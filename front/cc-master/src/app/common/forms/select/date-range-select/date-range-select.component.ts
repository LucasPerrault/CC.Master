import { Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IDateRange } from '@cc/common/date';
import { Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-date-range-select',
  templateUrl: './date-range-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DateRangeSelectComponent),
      multi: true,
    },
  ],
})
export class DateRangeSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  public onChange: (range: IDateRange) => void;
  public onTouch: () => void;

  public dateRangeSelected: FormGroup;
  public todayDate = new Date();

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
      this.dateRangeSelected.setValue(rangeSelectionUpdated);
      this.dateRangeSelected.updateValueAndValidity();
    }
  }

  public get startDate(): Date {
    return this.dateRangeSelected.get('startDate').value;
  }

  public get endDate(): Date {
    return this.dateRangeSelected.get('endDate').value;
  }
}
