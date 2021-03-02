import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ApiV3DateService } from '@cc/common/queries';

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

  public toHttpParams(filters: ILogsFilter): HttpParams {
    let params = new HttpParams();
    if (!!filters.environments.length) {
      const environmentIds = filters.environments.map(u => u.id);
      params = params.set(EnvironmentLogQueryParamKey.EnvironmentId, environmentIds.join(','));
    }

    if (!!filters.users.length) {
      const userIds = filters.users.map(u => u.id);
      params = params.set(EnvironmentLogQueryParamKey.UserId, userIds.join(','));
    }

    if (filters.isAnonymized !== null) {
      params = params.set(EnvironmentLogQueryParamKey.IsAnonymized, filters.isAnonymized.toString());
    }

    const createdOn = this.apiV3DateService.toApiDateRangeFormat(filters.createdOn);
    if (!!createdOn) {
      params = params.set(EnvironmentLogQueryParamKey.CreatedOn, createdOn);
    }

    if (!!filters.actions.length) {
      const actionIds = filters.actions.map(a => a.id);
      params = params.set(EnvironmentLogQueryParamKey.ActivityId, actionIds.join(','));
    }

    if (!!filters.domains.length) {
      const domainIds = filters.domains.map(d => d.id);
      params = params.set(EnvironmentLogQueryParamKey.EnvironmentDomain, domainIds.join(','));
    }

    return params;
  }
}
