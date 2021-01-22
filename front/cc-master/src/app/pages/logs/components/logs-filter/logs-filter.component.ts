import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { IFilterParams } from '@cc/common/filter';
import { BehaviorSubject } from 'rxjs';

import { EnvironmentLogFilterKeyEnum } from '../../enums';

@Component({
  selector: 'cc-logs-filter',
  templateUrl: './logs-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsFiltersComponent {
  @Output() public updateFilters: EventEmitter<IFilterParams> = new EventEmitter<IFilterParams>();

  public logsQueryParamsKeys = EnvironmentLogFilterKeyEnum;

  public isAnonymizedData = '';

  private queryFilters$: BehaviorSubject<IFilterParams> = new BehaviorSubject<IFilterParams>({});

  public update(key: string, value: string): void {
    const queryParamsKeys = Object.keys(this.queryFilters$.value);

    if (!value && !queryParamsKeys.includes(key)) {
      return;
    }

    if (!value && queryParamsKeys.includes(key)) {
      this.removeQueryParams(key);
      this.updateFilters.emit(this.queryFilters$.value);
      return;
    }

    this.addQueryParams(key, value);
    this.updateFilters.emit(this.queryFilters$.value);
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
