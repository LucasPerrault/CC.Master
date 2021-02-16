import { ChangeDetectionStrategy, Component, EventEmitter, Output } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params, Router } from '@angular/router';
import { apiV3ToDateRange, toApiDateRangeV3Format } from '@cc/common/queries';

import { ILogsFilter } from '../../models/logs-filter.interface';

enum EnvironmentLogRouterKeyEnum {
  UserId = 'user',
  ActivityId = 'activity',
  EnvironmentDomain = 'domains',
  EnvironmentId = 'environmentId',
  CreatedOn = 'createdOn',
  IsAnonymizedData = 'isAnonymizedData'
}

@Component({
  selector: 'cc-logs-filter',
  templateUrl: './logs-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsFiltersComponent {
  @Output() public updateFilters: EventEmitter<ILogsFilter> = new EventEmitter<ILogsFilter>();

  public logsFilter: ILogsFilter;

  constructor(private activatedRoute: ActivatedRoute, private router: Router) {
    this.initDefaultFilterValues(activatedRoute.snapshot);
  }

  public async updateAsync(): Promise<void> {
    this.updateFilters.emit(this.logsFilter);
    await this.updateRouterAsync(this.logsFilter);
  }

  private initDefaultFilterValues(route: ActivatedRouteSnapshot): void {
    this.logsFilter = this.toLogsFilter(route.queryParamMap);
    this.updateFilters.emit(this.logsFilter);
  }

  private async updateRouterAsync(logsFilter: ILogsFilter): Promise<void> {
    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: this.toRouterQueryParams(logsFilter),
      queryParamsHandling: 'merge',
    });
  }

  private toLogsFilter(routerParam: ParamMap): ILogsFilter {
    const isAnonymizedData = routerParam.has(EnvironmentLogRouterKeyEnum.IsAnonymizedData)
      ? routerParam.get(EnvironmentLogRouterKeyEnum.IsAnonymizedData)
      : '';

    return {
      environmentIds: this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.EnvironmentId),
      userIds: this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.UserId),
      actionIds: this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.ActivityId),
      domainIds: this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.EnvironmentDomain),
      createdOn: apiV3ToDateRange(routerParam.get(EnvironmentLogRouterKeyEnum.CreatedOn)),
      isAnonymizedData,
    };
  }

  private toRouterQueryParams(filter: ILogsFilter): Params {
    const createdOnRange = toApiDateRangeV3Format(filter.createdOn);

    return {
      [EnvironmentLogRouterKeyEnum.UserId]: !!filter.userIds.length ? filter.userIds.join(',') : null,
      [EnvironmentLogRouterKeyEnum.EnvironmentId]: !!filter.environmentIds.length ? filter.environmentIds.join(',') : null,
      [EnvironmentLogRouterKeyEnum.EnvironmentDomain]: !!filter.domainIds.length ? filter.domainIds.join(',') : null,
      [EnvironmentLogRouterKeyEnum.ActivityId]: !!filter.actionIds.length ? filter.actionIds.join(',') : null,
      [EnvironmentLogRouterKeyEnum.IsAnonymizedData]: !!filter.isAnonymizedData ? filter.isAnonymizedData : null,
      [EnvironmentLogRouterKeyEnum.CreatedOn]: !!createdOnRange ? createdOnRange : null,
    };
  }

  private convertToNumbers(param: ParamMap, key: EnvironmentLogRouterKeyEnum): number[] {
    if (!param.has(key) || !param.get(key) || param.get(key) === '') {
      return [];
    }

    return param.get(key)
      .split(',')
      .map(idToString => parseInt(idToString, 10));
  }
}
