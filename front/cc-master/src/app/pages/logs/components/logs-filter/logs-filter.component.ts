import { ChangeDetectionStrategy, Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FiltersService, IFilterParams } from '@cc/common/filters';

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
export class LogsFiltersComponent implements OnInit {
  @Output() public updateFilters: EventEmitter<IFilterParams> = new EventEmitter<IFilterParams>();

  public logsFilterParamsKeys = EnvironmentLogFilterKeyEnum;

  private filters: IFilterParams = {};

  constructor(private filterService: FiltersService) {
  }

  public ngOnInit(): void {
    this.updateFilters.emit(this.filters);
  }

  public update(key: string, value: string): void {
    const filtersUpdated = this.filterService.updateParams(this.filters, key, value);
    this.updateFilters.emit(filtersUpdated);
  }
}
