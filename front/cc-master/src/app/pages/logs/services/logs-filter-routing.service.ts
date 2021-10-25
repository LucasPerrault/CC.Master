import { Injectable } from '@angular/core';
import { ApiV3DateService } from '@cc/common/queries';
import {
  environmentActions,
  environmentDomains,
  EnvironmentsService,
  IEnvironment,
  IEnvironmentAction,
  IEnvironmentDomain,
} from '@cc/domain/environments';
import { IUser, UsersService } from '@cc/domain/users';
import { forkJoin, Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { ILogsFilter } from '../models/logs-filter.interface';
import { ILogsRoutingParams } from '../models/logs-routing-params.interface';

@Injectable()
export class LogsFilterRoutingService {

  constructor(
    private usersService: UsersService,
    private environmentsService: EnvironmentsService,
    private apiV3DateService: ApiV3DateService,
  ) { }

  public toLogsFilter$(routingParams: ILogsRoutingParams): Observable<ILogsFilter> {
    return forkJoin([
      this.getUsers$(routingParams.userIds),
      this.getEnvironments$(routingParams.environmentIds),
    ]).pipe(
      map(([users, environments]) => ({
        users,
        environments,
        actions: this.getEnvironmentActions(routingParams.actionIds),
        domains: this.getEnvironmentDomains(routingParams.domainIds),
        createdOn: this.apiV3DateService.toDateRange(routingParams.createdOn),
        isAnonymized: this.convertToNullableBoolean(routingParams.isAnonymized),
      }),
    ));
  }

  public toLogsRoutingParams(filters: ILogsFilter): ILogsRoutingParams {
    return {
      environmentIds: this.getSafeRoutingParams(filters.environments?.map(u => u.id)?.join(',')),
      domainIds: this.getSafeRoutingParams(filters.domains.map(d => d.id).join(',')),
      userIds: this.getSafeRoutingParams(filters.users.map(u => u.id).join(',')),
      actionIds: this.getSafeRoutingParams(filters.actions.map(a => a.id).join(',')),
      createdOn: this.getSafeRoutingParams(this.apiV3DateService.toApiDateRangeFormat(filters.createdOn)),
      isAnonymized: this.getSafeRoutingParams(filters.isAnonymized?.toString()),
    };
  }

  private getUsers$(userIdsToString: string): Observable<IUser[]> {
    const userIds = this.convertToNumbers(userIdsToString);
    if (!userIds.length) {
      return of([]);
    }

    return this.usersService.getUsersById$(userIds).pipe(take(1));
  }

  private getEnvironments$(environmentIdsToString: string): Observable<IEnvironment[]> {
    const environmentIds = this.convertToNumbers(environmentIdsToString);
    if (!environmentIds.length) {
      return of([]);
    }

    return this.environmentsService.getEnvironmentsByIds$(environmentIds).pipe(take(1));
  }

  private getEnvironmentActions(actionIdsToString: string): IEnvironmentAction[] {
    const actionIds = this.convertToNumbers(actionIdsToString);
    return environmentActions.filter(a => actionIds.includes(a.id));
  }

  private getEnvironmentDomains(domainIdsToString: string): IEnvironmentDomain[] {
    const domainIds = this.convertToNumbers(domainIdsToString);
    return environmentDomains.filter(d => domainIds.includes(d.id));
  }

  private getSafeRoutingParams(queryParams: string): string {
    if (!queryParams) {
      return;
    }

    return queryParams;
  }

  private convertToNumbers(values: string): number[] {
    if (!values) {
      return [];
    }
    return values.split(',').map(idToString => parseInt(idToString, 10));
  }

  private convertToNullableBoolean(value: string): boolean {
    if (value?.toLowerCase() === 'true') {
      return true;
    }

    if (value?.toLowerCase() === 'false') {
      return false;
    }

    return null;
  }
}
