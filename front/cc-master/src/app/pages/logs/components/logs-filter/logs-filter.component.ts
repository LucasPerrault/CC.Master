import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params, Router } from '@angular/router';
import { IPrincipal } from '@cc/aspects/principal';
import { apiV3ToDateRange, toApiDateRangeV3Format } from '@cc/common/queries';
import {
  environmentActions,
  environmentDomains,
  EnvironmentsService,
  IEnvironment,
  IEnvironmentAction,
  IEnvironmentDomain
} from '@cc/domain/environments';
import { UsersService } from '@cc/domain/users';
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

  public logsFilter: ILogsFilter = {
    users: [],
    environments: [],
    actions: [],
    createdOn: null,
    domains: [],
    isAnonymizedData: null,
  };

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private usersService: UsersService,
    private environmentsService: EnvironmentsService,
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
    this.toLogsFilterAsync(route.queryParamMap).then((l: ILogsFilter) => {
      this.logsFilter = l;
      this.updateFilters.emit(this.logsFilter);
    });
  }

  private async updateRouterAsync(logsFilter: ILogsFilter): Promise<void> {
    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: this.toRouterQueryParams(logsFilter),
      queryParamsHandling: 'merge',
    });
  }

  private async toLogsFilterAsync(routerParam: ParamMap): Promise<ILogsFilter> {
    const isAnonymizedData = routerParam.has(EnvironmentLogRouterKeyEnum.IsAnonymizedData)
      ? routerParam.get(EnvironmentLogRouterKeyEnum.IsAnonymizedData)
      : '';

    return {
      users: await this.getUsersWithRouterAsync(routerParam),
      environments: await this.getEnvironmentsWithRouterAsync(routerParam),
      actions: this.getActionsWithRouter(routerParam),
      domains: this.getDomainsWithRouter(routerParam),
      createdOn: apiV3ToDateRange(routerParam.get(EnvironmentLogRouterKeyEnum.CreatedOn)),
      isAnonymizedData,
    } as ILogsFilter;
  }

  private async getUsersWithRouterAsync(routerParam: ParamMap): Promise<IPrincipal[]> {
    const userIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.UserId);
    if (!userIds.length) {
      return [];
    }

    return await this.usersService.getUsersById$(userIds)
      .pipe(take(1))
      .toPromise();
  }

  private async getEnvironmentsWithRouterAsync(routerParam: ParamMap): Promise<IEnvironment[]> {
    const environmentIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.EnvironmentId);
    if (!environmentIds.length) {
      return [];
    }

    return await this.environmentsService.getEnvironmentsById$(environmentIds)
      .pipe(take(1))
      .toPromise();
  }

  private getActionsWithRouter(routerParam: ParamMap): IEnvironmentAction[] {
    const actionIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.ActivityId);
    if (!actionIds.length) {
      return [];
    }

    return environmentActions.filter(a => actionIds.includes(a.id));
  }

  private getDomainsWithRouter(routerParam: ParamMap): IEnvironmentDomain[] {
    const domainIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.EnvironmentDomain);
    if (!domainIds.length) {
      return [];
    }

    return environmentDomains.filter(d => domainIds.includes(d.id));
  }

  private toRouterQueryParams(filter: ILogsFilter): Params {
    const createdOnRange = toApiDateRangeV3Format(filter.createdOn);
    const action = !!filter.actions.length ? filter.actions.map(a => a.id).join(',') : null;
    const domainIds = !!filter.domains.length ? filter.domains.map(d => d.id).join(',') : null;

    return {
      [EnvironmentLogRouterKeyEnum.UserId]: !!filter.users.length ? filter.users.map(u => u.id).join(',') : null,
      [EnvironmentLogRouterKeyEnum.EnvironmentId]: !!filter.environments.length ? filter.environments.map(e => e.id).join(',') : null,
      [EnvironmentLogRouterKeyEnum.EnvironmentDomain]: domainIds,
      [EnvironmentLogRouterKeyEnum.ActivityId]: action,
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
