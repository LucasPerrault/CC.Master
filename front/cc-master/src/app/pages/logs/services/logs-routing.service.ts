import { Injectable } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { ILogsRoutingParams } from '../models/logs-routing-params.interface';

enum LogsRoutingKey {
  UserId = 'users',
  ActivityId = 'activities',
  EnvironmentDomain = 'domains',
  EnvironmentId = 'environments',
  CreatedOn = 'createdOn',
  IsAnonymized = 'isAnonymized'
}

@Injectable()
export class LogsRoutingService {
  constructor(private activatedRoute: ActivatedRoute, private router: Router) {
  }

  public getLogsRoutingParams(): ILogsRoutingParams {
    const params = this.activatedRoute.snapshot.queryParamMap;
    return {
      userIds: params.get(LogsRoutingKey.UserId),
      environmentIds: params.get(LogsRoutingKey.EnvironmentId),
      domainIds: params.get(LogsRoutingKey.EnvironmentDomain),
      actionIds: params.get(LogsRoutingKey.ActivityId),
      createdOn: params.get(LogsRoutingKey.CreatedOn),
      isAnonymized : params.get(LogsRoutingKey.IsAnonymized),
    };
  }

  public async updateRouterAsync(logsParam: ILogsRoutingParams): Promise<void> {
    const queryParams = {
      [LogsRoutingKey.UserId]: logsParam.userIds,
      [LogsRoutingKey.EnvironmentId]: logsParam.environmentIds,
      [LogsRoutingKey.EnvironmentDomain]: logsParam.domainIds,
      [LogsRoutingKey.ActivityId]: logsParam.actionIds,
      [LogsRoutingKey.CreatedOn]: logsParam.createdOn,
      [LogsRoutingKey.IsAnonymized]: logsParam.isAnonymized,
    };

    await this.router.navigate([], {
      queryParams,
      queryParamsHandling: 'merge',
      relativeTo: this.activatedRoute,
    });
  }
}
