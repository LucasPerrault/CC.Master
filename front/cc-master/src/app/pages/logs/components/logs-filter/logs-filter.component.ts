import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

import { EnvironmentLogFilterKeyEnum } from '../../enums';
import { IHttpQueryParams } from '../../../../common/queries';

@Component({
  selector: 'cc-logs-filter',
  templateUrl: './logs-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsFiltersComponent {
  @Output() public updateQueryFilters: EventEmitter<IHttpQueryParams> = new EventEmitter<IHttpQueryParams>();

  public logsQueryParamsKeys = EnvironmentLogFilterKeyEnum;

  public isAnonymizedData: string = '';

  private _queryFilters$: BehaviorSubject<IHttpQueryParams> = new BehaviorSubject<IHttpQueryParams>({});

  public updateFilters(key: string, value: string): void {
    const queryParamsKeys = Object.keys(this._queryFilters$.value);

    if (!value && !queryParamsKeys.includes(key)) {
      return;
    }

    !value && queryParamsKeys.includes(key)
      ? this.removeQueryParams(key, value)
      : this.addQueryParams(key, value)

    this.updateQueryFilters.emit(this._queryFilters$.value);
  }

  private addQueryParams(key: string, value: string) {
    this._queryFilters$.next({
      ...this._queryFilters$.value,
      [key]: value
    });
  }

  private removeQueryParams(key: string, value: string) {
    const currentQueryFilters = this._queryFilters$.value;
    delete currentQueryFilters[key];
    this._queryFilters$.next(currentQueryFilters);
  }
}
