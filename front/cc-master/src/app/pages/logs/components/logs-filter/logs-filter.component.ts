import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

import { ILogsFilter } from '../../models/logs-filter.interface';

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
export class LogsFiltersComponent implements ControlValueAccessor {
  public onChange: (logsFilter: ILogsFilter) => void;
  public onTouch: () => void;

  public logsFilter: ILogsFilter = {
    users: [],
    environments: [],
    actions: [],
    createdOn: {
      startDate: null,
      endDate: null,
    },
    domains: [],
    isAnonymized: null,
  };

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

    this.logsFilter = logsFilter;
  }

  public update(): void {
    this.onChange(this.logsFilter);
  }
}
