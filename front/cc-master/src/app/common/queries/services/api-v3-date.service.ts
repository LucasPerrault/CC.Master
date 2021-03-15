import { formatDate } from '@angular/common';
import { Inject, Injectable, LOCALE_ID } from '@angular/core';
import { IDateRange } from '@cc/common/date';

@Injectable()
export class ApiV3DateService {
  private readonly apiV3DateFormat = 'yyyy-MM-dd';
  private readonly apiBetweenKeyword = 'between';
  private readonly apiSinceKeyword = 'since';
  private readonly apiUntilKeyword = 'until';

  constructor(@Inject(LOCALE_ID) private locale) {
  }

  public toApiV3DateFormat(date: Date): string {
    if (!date || isNaN(date.getTime())) {
      return '';
    }

    return formatDate(date, this.apiV3DateFormat, this.locale);
  }

  public toApiDateRangeFormat = (dateRange: IDateRange): string => {
    if (!dateRange.startDate && !dateRange.endDate) {
      return '';
    }
    const startDateFormat = this.toApiV3DateFormat(dateRange.startDate);
    const endDateFormat = this.toApiV3DateFormat(dateRange.endDate);

    if (!endDateFormat) {
      return `${this.apiSinceKeyword},${startDateFormat}`;
    }
    if (!startDateFormat) {
      return `${this.apiUntilKeyword},${endDateFormat}`;
    }

    return `${this.apiBetweenKeyword},${startDateFormat},${endDateFormat}`;
  };

  public toDateRange = (dateToString: string): IDateRange => {
    const defaultDateRange = {
      startDate: null,
      endDate: null,
    };

    if (!dateToString) {
      return defaultDateRange;
    }

    const dateParams = dateToString.split(',');
    const keyword = dateParams.shift();
    switch (keyword) {
      case this.apiBetweenKeyword:
        return {
          startDate: new Date(dateParams.shift()),
          endDate: new Date(dateParams.shift()),
        };
      case this.apiSinceKeyword:
        return {
          startDate: new Date(dateParams.shift()),
          endDate: null,
        };
      case this.apiUntilKeyword:
        return {
          startDate: null,
          endDate: new Date(dateParams.shift()),
        };
      default:
        return defaultDateRange;
    }
  };

}
