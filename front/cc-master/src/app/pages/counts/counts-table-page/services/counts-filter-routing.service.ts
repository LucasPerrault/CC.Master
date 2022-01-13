import { Injectable } from '@angular/core';
import { IDateRange } from '@cc/common/date';
import { ApiV3DateService } from '@cc/common/queries';
import { ClientsService, IClient } from '@cc/domain/billing/clients';
import { DistributorsService, IDistributor } from '@cc/domain/billing/distributors';
import { IOffer, OffersService, ProductsService } from '@cc/domain/billing/offers';
import { EnvironmentGroupsService } from '@cc/domain/environments';
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

export interface ICountsRoutingParams {
  countPeriod: string;
  clientIds: string;
  distributorIds: string;
  offerIds: string;
  environmentGroupIds: string;
  productIds: string;
  columns: string;
}

@Injectable()
export class CountsFilterRoutingService {

  constructor(
    private offersService: OffersService,
    private clientsService: ClientsService,
    private productsService: ProductsService,
    private distributorsService: DistributorsService,
    private environmentGroupsService: EnvironmentGroupsService,
    private apiV3DateService: ApiV3DateService,
  ) { }

  public toColumnsSelected(routingParams: ICountsRoutingParams): ICountAdditionalColumn[] {
    const defaultColumnIds = defaultColumnsDisplayed.map(c => c.id);
    const columnsDisplayed = routingParams?.columns?.split(',') ?? defaultColumnIds;

    return countAdditionalColumns.filter(column => !!columnsDisplayed.find(c => c === column.id));
  }

  public toFilter$(routingParams: ICountsRoutingParams): Observable<ICountsFilterForm> {
    return forkJoin([
      this.getOffers$(routingParams.offerIds),
      this.getClients$(routingParams.clientIds),
      this.getProducts$(routingParams.productIds),
      this.getDistributors$(routingParams.distributorIds),
      this.getEnvironmentGroups$(routingParams.environmentGroupIds),
    ]).pipe(
      map(([offers, clients, products, distributors, environmentGroups]) => ({
        countPeriod: this.getCountPeriod(routingParams?.countPeriod),
        offers,
        clients,
        products,
        distributors,
        environmentGroups,
      }),
    ));
  }

  public toRoutingParams(filters: ICountsFilterForm, columns: CountAdditionalColumn[]): ICountsRoutingParams {
    return {
      countPeriod: this.getSafeRoutingParams(this.apiV3DateService.toApiDateRangeFormat(filters?.countPeriod)),
      clientIds: this.getSafeRoutingParams(filters.clients?.map(c => c.id).join(',')),
      distributorIds: this.getSafeRoutingParams(filters.distributors?.map(d => d.id).join(',')),
      offerIds: this.getSafeRoutingParams(filters.offers?.map(o => o.id).join(',')),
      environmentGroupIds: this.getSafeRoutingParams(filters.environmentGroups?.map(e => e.id).join(',')),
      productIds: this.getSafeRoutingParams(filters.products?.map(p => p.id).join(',')),
      columns: this.getSafeRoutingParams(columns.join(',')),
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
