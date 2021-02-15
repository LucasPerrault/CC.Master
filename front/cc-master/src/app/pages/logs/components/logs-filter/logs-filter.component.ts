import { ChangeDetectionStrategy, Component, EventEmitter, OnDestroy, OnInit, Output } from '@angular/core';
import { FiltersService, IFilterParams } from '@cc/common/filters';
import { BehaviorSubject, Subject } from 'rxjs';
import { debounceTime, takeUntil } from 'rxjs/operators';

enum EnvironmentLogFilterKeyEnum {
  UserId = 'userId',
  ActivityId = 'activityId',
  EnvironmentDomain = 'environment.domain',
  EnvironmentSubDomain = 'environment.subdomain',
  CreatedOn = 'createdOn',
  IsAnonymizedData = 'isAnonymizedData'
}

@Component({
  selector: 'cc-logs-filter',
  templateUrl: './logs-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsFiltersComponent implements OnInit, OnDestroy {
  @Output() public updateFilters: EventEmitter<IFilterParams> = new EventEmitter<IFilterParams>();

  public logsFilterParamsKeys = EnvironmentLogFilterKeyEnum;

  private filters: BehaviorSubject<IFilterParams> = new BehaviorSubject<IFilterParams>({});
  private destroy: Subject<void> = new Subject<void>();

  constructor(private filterService: FiltersService) {
  }

  public ngOnInit(): void {
    this.filters.pipe(debounceTime(300), takeUntil(this.destroy))
      .subscribe(f => this.updateFilters.emit(f));
  }

  public ngOnDestroy(): void {
    this.destroy.next();
    this.destroy.complete();
  }

  public update(key: string, value: string): void {
    const hasEmptyValueAndExistingKey = !value && Object.keys(this.filters.value);
    const filtersUpdated = hasEmptyValueAndExistingKey
      ? this.filterService.removeKey(this.filters.value, key)
      : this.filterService.updateParam(this.filters.value, key, value);

    this.filters.next(filtersUpdated);
  }
}
