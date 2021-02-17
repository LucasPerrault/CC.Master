import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params, Router } from '@angular/router';
import { IPrincipal } from '@cc/aspects/principal';
import { apiV3ToDateRange, toApiDateRangeV3Format } from '@cc/common/queries';
import { UsersService } from '@cc/domain/users';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';

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
})
export class LogsFiltersComponent implements OnInit {
  @Output() public updateFilters: EventEmitter<ILogsFilter> = new EventEmitter<ILogsFilter>();

  public logsFilter: ILogsFilter;

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private usersService: UsersService,
  ) {
  }

  public ngOnInit(): void {
    this.initDefaultFilterValues(this.activatedRoute.snapshot);
  }

  public async updateAsync(): Promise<void> {
    this.updateFilters.emit(this.logsFilter);
    await this.updateRouterAsync(this.logsFilter);
  }

  private initDefaultFilterValues(route: ActivatedRouteSnapshot): void {
    this.logsFilter = this.toLogsFilter(route.queryParamMap);

    this.setUsersWithRouter(route.queryParamMap);
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
      users: [],
      actionIds: this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.ActivityId),
      domainIds: this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.EnvironmentDomain),
      createdOn: apiV3ToDateRange(routerParam.get(EnvironmentLogRouterKeyEnum.CreatedOn)),
      isAnonymizedData,
    } as ILogsFilter;
  }

  private setUsersWithRouter(routerParam: ParamMap): void {
    const userIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.UserId);
    if (!userIds.length) {
      return;
    }

    this.usersService.getUsersById$(userIds)
      .pipe(take(1))
      .subscribe(u => this.logsFilter.users = u);
  }

  private toRouterQueryParams(filter: ILogsFilter): Params {
    const createdOnRange = toApiDateRangeV3Format(filter.createdOn);

    return {
      [EnvironmentLogRouterKeyEnum.UserId]: !!filter.users.length ? filter.users.map(u => u.id).join(',') : null,
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
