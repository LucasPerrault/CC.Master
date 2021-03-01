import { Component, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ActivatedRoute, ActivatedRouteSnapshot, ParamMap, Params, Router } from '@angular/router';
import { IPrincipal } from '@cc/aspects/principal';
import { apiV3ToDateRange, toApiDateRangeV3Format } from '@cc/common/queries';
import {
  environmentActions,
  environmentDomains,
  EnvironmentsService,
  IEnvironment,
  IEnvironmentAction,
  IEnvironmentDomain,
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
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => LogsFiltersComponent),
      multi: true,
    },
  ],
})
export class LogsFiltersComponent implements OnInit, ControlValueAccessor {
  public onChange: (logsFilter: ILogsFilter) => void;
  public onTouch: () => void;

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

    if (!this.isEqual(this.logsFilter, logsFilter)) {
      this.logsFilter = logsFilter;
    }
  }

  public async updateAsync(): Promise<void> {
    this.onChange(this.logsFilter);
    await this.updateRouterAsync(this.logsFilter);
  }

  private isEqual(a: ILogsFilter, b: ILogsFilter): boolean {
    return a === b;
  }

  private initDefaultFilterValues(route: ActivatedRouteSnapshot): void {
    this.toLogsFilterAsync(route.queryParamMap).then((l: ILogsFilter) => {
      this.logsFilter = l;
      this.onChange(this.logsFilter);
    });
  }

  private async updateRouterAsync(logsFilter: ILogsFilter): Promise<void> {
    await this.router.navigate([], {
      relativeTo: this.activatedRoute,
      queryParams: this.toRouterQueryParams(logsFilter),
      queryParamsHandling: 'merge',
    });
  }

  private toRouterQueryParams(filter: ILogsFilter): Params {
    const userIds = !!filter.users.length ? filter.users.map(u => u.id).join(',') : null;
    const environmentIds = !!filter.environments.length ? filter.environments.map(e => e.id).join(',') : null;
    const domainIds = !!filter.domains.length ? filter.domains.map(d => d.id).join(',') : null;
    const actionIds = !!filter.actions.length ? filter.actions.map(a => a.id).join(',') : null;
    const createdOnRange = toApiDateRangeV3Format(filter.createdOn);

    return {
      [EnvironmentLogRouterKeyEnum.UserId]: userIds,
      [EnvironmentLogRouterKeyEnum.EnvironmentId]: environmentIds,
      [EnvironmentLogRouterKeyEnum.EnvironmentDomain]: domainIds,
      [EnvironmentLogRouterKeyEnum.ActivityId]: actionIds,
      [EnvironmentLogRouterKeyEnum.IsAnonymizedData]: !!filter.isAnonymizedData ? filter.isAnonymizedData : null,
      [EnvironmentLogRouterKeyEnum.CreatedOn]: !!createdOnRange ? createdOnRange : null,
    };
  }

  private async toLogsFilterAsync(routerParam: ParamMap): Promise<ILogsFilter> {
    const isAnonymizedData = routerParam.has(EnvironmentLogRouterKeyEnum.IsAnonymizedData)
      ? routerParam.get(EnvironmentLogRouterKeyEnum.IsAnonymizedData)
      : '';

    return {
      users: await this.getUsersAsync(routerParam),
      environments: await this.getEnvironmentsAsync(routerParam),
      actions: this.getEnvironmentActions(routerParam),
      domains: this.getEnvironmentDomains(routerParam),
      createdOn: apiV3ToDateRange(routerParam.get(EnvironmentLogRouterKeyEnum.CreatedOn)),
      isAnonymizedData,
    } as ILogsFilter;
  }

  private async getUsersAsync(routerParam: ParamMap): Promise<IPrincipal[]> {
    const userIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.UserId);
    if (!userIds.length) {
      return [];
    }

    return await this.usersService.getUsersById$(userIds)
      .pipe(take(1))
      .toPromise();
  }

  private async getEnvironmentsAsync(routerParam: ParamMap): Promise<IEnvironment[]> {
    const environmentIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.EnvironmentId);
    if (!environmentIds.length) {
      return [];
    }

    return await this.environmentsService.getEnvironmentsById$(environmentIds)
      .pipe(take(1))
      .toPromise();
  }

  private getEnvironmentActions(routerParam: ParamMap): IEnvironmentAction[] {
    const actionIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.ActivityId);
    if (!actionIds.length) {
      return [];
    }

    return environmentActions.filter(a => actionIds.includes(a.id));
  }

  private getEnvironmentDomains(routerParam: ParamMap): IEnvironmentDomain[] {
    const domainIds = this.convertToNumbers(routerParam, EnvironmentLogRouterKeyEnum.EnvironmentDomain);
    if (!domainIds.length) {
      return [];
    }

    return environmentDomains.filter(d => domainIds.includes(d.id));
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
