import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { IHttpQueryParams } from '@cc/common/queries';
import { BehaviorSubject } from 'rxjs';

import { EnvironmentLogFilterKeyEnum } from '../../enums';

@Component({
  selector: 'cc-logs-filter',
  templateUrl: './logs-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsFiltersComponent {
  @Output() public updateQueryFilters: EventEmitter<IHttpQueryParams> = new EventEmitter<IHttpQueryParams>();

  public logsQueryParamsKeys = EnvironmentLogFilterKeyEnum;

  public isAnonymizedData = '';

  private queryFilters$: BehaviorSubject<IHttpQueryParams> = new BehaviorSubject<IHttpQueryParams>({});

  public updateFilters(key: string, value: string): void {
    const queryParamsKeys = Object.keys(this.queryFilters$.value);

    if (!value && !queryParamsKeys.includes(key)) {
      return;
    }

    if (!value && queryParamsKeys.includes(key)) {
      this.removeQueryParams(key);
      this.updateQueryFilters.emit(this.queryFilters$.value);
      return;
    }

    this.addQueryParams(key, value);
    this.updateQueryFilters.emit(this.queryFilters$.value);
  }

  private addQueryParams(key: string, value: string) {
    this.queryFilters$.next({
      ...this.queryFilters$.value,
      [key]: value,
    });
  }

  private removeQueryParams(key: string) {
    const currentQueryFilters = this.queryFilters$.value;
    delete currentQueryFilters[key];
    this.queryFilters$.next(currentQueryFilters);
  }
}
