import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { IDateRange } from '@cc/common/date';

export enum DateRangeKeys {
  StartDate = 'startDate',
  EndDate = 'endDate',
}

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
export class DateRangeSelectComponent implements ControlValueAccessor {
  public onChange: (range: IDateRange) => void;
  public onTouch: () => void;

  public dateRangeKeys = DateRangeKeys;
  public dateRangeSelected: IDateRange = {
    startDate: null,
    endDate: null,
  };

  public todayDate = new Date();

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(rangeSelectionUpdated: IDateRange): void {
    if (rangeSelectionUpdated !== this.dateRangeSelected && rangeSelectionUpdated != null) {
      this.dateRangeSelected = rangeSelectionUpdated;
    }
  }

  public updateDateRange(key: DateRangeKeys, date: Date): void {
    this.dateRangeSelected = {
      ...this.dateRangeSelected,
      [key]: date,
    };

    this.onChange(this.dateRangeSelected);
  }
}
