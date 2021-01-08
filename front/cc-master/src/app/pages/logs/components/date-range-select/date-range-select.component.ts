import { Component, EventEmitter, Output } from '@angular/core';
import { LuNativeDateAdapter } from '@lucca-front/ng/core';

import { ApiDateFormat } from '../../queries';

@Component({
  selector: 'cc-date-range-select',
  templateUrl: './date-range-select.component.html',
})
export class DateRangeSelectComponent {
  @Output() public dateRangeToString: EventEmitter<string> = new EventEmitter<string>();

  public startDate;
  public endDate;
  public todayDate = new Date();

  constructor(private adapter: LuNativeDateAdapter) {}

  public updateDatesSelected(startDate: Date, endDate: Date) {
    const dateRangeToApiQueryString = this.getDateRangeToQueryString(startDate, endDate);
    this.dateRangeToString.emit(dateRangeToApiQueryString);
  }

  public getDateRangeToQueryString(startDate: Date, endDate: Date): string {
    if (!startDate && !endDate) {
      return;
    }

    const startDateFormat = this.getDateToApiQueryString(startDate);
    const endDateFormat = this.getDateToApiQueryString(endDate);

    if (!endDateFormat) {
      return `since,${startDateFormat}`;
    }
    if (!startDateFormat) {
      return `until,${endDateFormat}`;
    }

    return `between,${startDateFormat},${endDateFormat}`;
  }

  public getDateToApiQueryString(date: Date): string {
    if (!date) {
      return;
    }

    return this.adapter.format(date, ApiDateFormat);
  }
}
