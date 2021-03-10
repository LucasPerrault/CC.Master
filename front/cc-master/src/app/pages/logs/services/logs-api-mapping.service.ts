import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IDateRange } from '@cc/common/date';
import { ApiV3DateService, apiV3SortToHttpParams, toApiV3SortParams } from '@cc/common/queries';
import { ISortParams } from '@cc/common/sort';
import { IEnvironment, IEnvironmentAction, IEnvironmentDomain } from '@cc/domain/environments';
import { IUser } from '@cc/domain/users';

import { ILogsFilter } from '../models/logs-filter.interface';

enum EnvironmentLogQueryParamKey {
  UserId = 'userId',
  ActivityId = 'activityId',
  EnvironmentDomain = 'environment.domain',
  EnvironmentId = 'environmentId',
  CreatedOn = 'createdOn',
  IsAnonymized = 'isAnonymizedData'
}

@Injectable()
export class LogsApiMappingService {

  constructor(private apiV3DateService: ApiV3DateService) { }

  public toHttpParams(filters: ILogsFilter, sortParams: ISortParams): HttpParams {
    let params = new HttpParams();
    params = this.setSortParams(params, sortParams);
    return this.setLogsFilter(params, filters);
  }

  private setSortParams(params: HttpParams, sortParams: ISortParams): HttpParams {
    if (!sortParams) {
      return params;
    }

    const apiV3SortParams = toApiV3SortParams(sortParams);
    return apiV3SortToHttpParams(params, apiV3SortParams);
  }

  private setLogsFilter(params: HttpParams, filters: ILogsFilter): HttpParams {
    if (!filters) {
      return params;
    }

    params = this.setEnvironments(params, filters.environments);
    params = this.setUsers(params, filters.users);
    params = this.setActions(params, filters.actions);
    params = this.setDomains(params, filters.domains);
    params = this.setIsAnonymized(params, filters.isAnonymized);
    return this.setCreatedOn(params, filters.createdOn);
  }

  private setEnvironments(params: HttpParams, environments: IEnvironment[]): HttpParams {
    if (!environments.length) {
      return params.delete(EnvironmentLogQueryParamKey.EnvironmentId);
    }

    const environmentIds = environments.map(u => u.id);
    return params.set(EnvironmentLogQueryParamKey.EnvironmentId, environmentIds.join(','));
  }

  private setUsers(params: HttpParams, users: IUser[]): HttpParams {
    if (!users.length) {
      return params.delete(EnvironmentLogQueryParamKey.UserId);
    }

    const usersIds = users.map(u => u.id);
    return params.set(EnvironmentLogQueryParamKey.UserId, usersIds.join(','));
  }

  private setActions(params: HttpParams, actions: IEnvironmentAction[]): HttpParams {
    if (!actions.length) {
      return params.delete(EnvironmentLogQueryParamKey.ActivityId);
    }

    const actionsIds = actions.map(u => u.id);
    return params.set(EnvironmentLogQueryParamKey.ActivityId, actionsIds.join(','));
  }

  private setDomains(params: HttpParams, domains: IEnvironmentDomain[]): HttpParams {
    if (!domains.length) {
      return params.delete(EnvironmentLogQueryParamKey.EnvironmentDomain);
    }

    const domainsIds = domains.map(u => u.id);
    return params.set(EnvironmentLogQueryParamKey.EnvironmentDomain, domainsIds.join(','));
  }

  private setIsAnonymized(params: HttpParams, isAnonymized?: boolean): HttpParams {
    if (isAnonymized === null) {
      return params.delete(EnvironmentLogQueryParamKey.IsAnonymized);
    }

    return params.set(EnvironmentLogQueryParamKey.IsAnonymized, isAnonymized.toString());
  }

  private setCreatedOn(params: HttpParams, createdOn: IDateRange): HttpParams {
    if (!createdOn.startDate && !createdOn.endDate) {
      return params.delete(EnvironmentLogQueryParamKey.CreatedOn);
    }

    const apiV3CreatedOn = this.apiV3DateService.toApiDateRangeFormat(createdOn);
    return params.set(EnvironmentLogQueryParamKey.CreatedOn, apiV3CreatedOn);
  }
}
