import { IDateRange } from '@cc/common/date';

export const apiV3BetweenKeyword = 'between';

export const apiSinceKeyword = 'since';
export const apiUntilKeyword = 'until';

export const toApiDateRangeV3Format = (dateRange: IDateRange): string => {
  if (!dateRange.startDate && !dateRange.endDate) {
    return '';
  }
  const startDateFormat = toApiFormat(dateRange.startDate);
  const endDateFormat = toApiFormat(dateRange.endDate);

  if (!endDateFormat) {
    return `${apiSinceKeyword},${startDateFormat}`;
  }
  if (!startDateFormat) {
    return `${apiUntilKeyword},${endDateFormat}`;
  }

  return `${apiV3BetweenKeyword},${startDateFormat},${endDateFormat}`;
};

export const toApiFormat = (date: Date): string => {
  if (!date || isNaN(date.getTime())) {
    return '';
  }

  const dateWithAnyTimezoneDifference = new Date(date.getTime() - (date.getTimezoneOffset() * 60000 ));
  const dateToISOString = dateWithAnyTimezoneDifference.toISOString();
  return dateToISOString.slice(0, 10);
};

export const apiV3ToDateRange = (dateToString: string): IDateRange => {
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
    case apiV3BetweenKeyword:
      return {
        startDate: new Date(dateParams.shift()),
        endDate: new Date(dateParams.shift()),
      };
    case apiSinceKeyword:
      return {
        startDate: new Date(dateParams.shift()),
        endDate: null,
      };
    case apiUntilKeyword:
        return {
          startDate: null,
          endDate: new Date(dateParams.shift()),
        };
    default:
      return defaultDateRange;
  }
};
