import { Injectable } from '@angular/core';
import { IDateRange } from '@cc/common/date';
import { ApiV3DateService } from '@cc/common/queries';
import { ClientsService, IClient } from '@cc/domain/billing/clients';
import { DistributorsService, IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, OffersService, ProductsService } from '@cc/domain/billing/offers';
import { EnvironmentGroupsService, EnvironmentsService, IEnvironment } from '@cc/domain/environments';
import { IEnvironmentGroup } from '@cc/domain/environments/models/environment-group.interface';
import { endOfMonth, startOfMonth, subMonths } from 'date-fns';
import { forkJoin, Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';

import {
  CountAdditionalColumn,
  countAdditionalColumns,
  defaultColumnsDisplayed,
  ICountAdditionalColumn,
} from '../components/count-additional-column-select/count-additional-column.enum';
import { ICountsFilterForm } from '../models/counts-filter-form.interface';
import { ICountsQueryParams } from '../models/counts-query-params.interface';

@Injectable()
export class CountsFilterRoutingService {

  constructor(
    private offersService: OffersService,
    private clientsService: ClientsService,
    private productsService: ProductsService,
    private distributorsService: DistributorsService,
    private environmentGroupsService: EnvironmentGroupsService,
    private environmentsService: EnvironmentsService,
    private apiV3DateService: ApiV3DateService,
  ) { }

  public toColumnsSelected(routingParams: ICountsQueryParams): ICountAdditionalColumn[] {
    const defaultColumnIds = defaultColumnsDisplayed.map(c => c.id);
    const columnsDisplayed = routingParams?.column?.split(',') ?? defaultColumnIds;

    return countAdditionalColumns.filter(column => !!columnsDisplayed.find(c => c === column.id));
  }

  public toFilter$(routingParams: ICountsQueryParams): Observable<ICountsFilterForm> {
    return forkJoin([
      this.getOffers$(routingParams.offerId),
      this.getClients$(routingParams.clientId),
      this.getProducts$(routingParams.productId),
      this.getDistributors$(routingParams.distributorId),
      this.getEnvironmentGroups$(routingParams.environmentGroupId),
      this.getEnvironments$(routingParams.environmentId),
    ]).pipe(
      map(([offers, clients, products, distributors, environmentGroups, environments]) => ({
        countPeriod: this.getCountPeriod(routingParams?.countPeriod),
        offers,
        clients,
        products,
        distributors,
        environmentGroups,
        environments,
      }),
    ));
  }

  public toRoutingParams(filters: ICountsFilterForm, columns: CountAdditionalColumn[]): ICountsQueryParams {
    return {
      countPeriod: this.getSafeRoutingParams(this.apiV3DateService.toApiDateRangeFormat(filters?.countPeriod)),
      clientId: this.getSafeRoutingParams(filters.clients?.map(c => c.id).join(',')),
      distributorId: this.getSafeRoutingParams(filters.distributors?.map(d => d.id).join(',')),
      offerId: this.getSafeRoutingParams(filters.offers?.map(o => o.id).join(',')),
      environmentGroupId: this.getSafeRoutingParams(filters.environmentGroups?.map(e => e.id).join(',')),
      productId: this.getSafeRoutingParams(filters.products?.map(p => p.id).join(',')),
      column: this.getSafeRoutingParams(columns.join(',')),
      environmentId: this.getSafeRoutingParams(filters?.environments?.map(e => e.id).join(',')),
    };
  }

  private getCountPeriod(countPeriod: string): IDateRange {
    const range = this.apiV3DateService.toDateRange(countPeriod);
    const defaultRange = {
      startDate: subMonths(startOfMonth(Date.now()), 1),
      endDate: subMonths(endOfMonth(Date.now()), 1),
    };

    return !!range.startDate || !!range.endDate ? range : defaultRange;
  }

  private getClients$(idsToString: string): Observable<IClient[]> {
    const clientIds = this.convertToNumbers(idsToString);
    return !!clientIds.length
      ? this.clientsService.getClientsById$(clientIds).pipe(take(1))
      : of([]);
  }

  private getProducts$(idsToString: string): Observable<IClient[]> {
    const productIds = this.convertToNumbers(idsToString);
    return !!productIds.length
      ? this.productsService.getProductsById$(productIds).pipe(take(1))
      : of([]);
  }

  private getOffers$(idsToString: string): Observable<IOffer[]> {
    const offerIds = this.convertToNumbers(idsToString);
    return !!offerIds.length
      ? this.offersService.getOffersById$(offerIds).pipe(take(1))
      : of([]);
  }

  private getDistributors$(ids: string): Observable<IDistributor[]> {
    const distributorIds = !!ids ? ids.split(',') : [];
    return !!distributorIds.length
      ? this.distributorsService.getDistributorsById$(distributorIds).pipe(take(1))
      : of([]);
  }

  private getEnvironmentGroups$(idsToString: string): Observable<IEnvironmentGroup[]> {
    const groupIds = this.convertToNumbers(idsToString);
    return !!groupIds.length
      ? this.environmentGroupsService.getEnvironmentGroupsByIds$(groupIds).pipe(take(1))
      : of([]);
  }

  private getEnvironments$(idsToString: string): Observable<IEnvironment[]> {
    const environmentIds = this.convertToNumbers(idsToString);
    return !!environmentIds.length
      ? this.environmentsService.getEnvironmentsByIds$(environmentIds).pipe(take(1))
      : of([]);
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
}
