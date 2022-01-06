import { Injectable } from '@angular/core';
import { ApiV3DateService } from '@cc/common/queries';
import { ClientsService, getBillingEntity, IBillingEntity, IClient } from '@cc/domain/billing/clients';
import { DistributorsService, IDistributor } from '@cc/domain/billing/distributors';
import { EstablishmentsService, IEstablishment } from '@cc/domain/billing/establishments';
import { IOffer, IProduct, OffersService, ProductsService } from '@cc/domain/billing/offers';
import { EnvironmentsService, IEnvironment } from '@cc/domain/environments';
import { forkJoin, Observable, of } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { DistributorFilter } from '../../common/distributor-filter-button-group';
import {
  ContractAdditionalColumn,
  contractAdditionalColumns,
  defaultColumnsDisplayed,
  IContractAdditionalColumn,
} from '../constants/contract-additional-column.enum';
import { contractEstablishmentsHealth } from '../constants/contract-establishment-health.enum';
import { ContractState, contractStates } from '../constants/contract-state.enum';
import { IContractEstablishmentHealth } from '../models/contract-establishment-health.interface';
import { IContractState } from '../models/contract-state.interface';
import { IContractsFilter } from '../models/contracts-filter.interface';
import { IContractsRoutingParams } from '../models/contracts-routing-params.interface';

@Injectable()
export class ContractsFilterRoutingService {

  constructor(
    private clientsService: ClientsService,
    private productsService: ProductsService,
    private offersService: OffersService,
    private distributorsService: DistributorsService,
    private environmentsService: EnvironmentsService,
    private establishmentsService: EstablishmentsService,
    private apiV3DateService: ApiV3DateService,
  ) { }

  public toContractColumnsSelected(routingParams: IContractsRoutingParams): IContractAdditionalColumn[] {
    const columnsDisplayed = !!routingParams.columns
      ? routingParams.columns.split(',')
      : defaultColumnsDisplayed;

    return contractAdditionalColumns.filter(column => !!columnsDisplayed.find(c => c === column.id));
  }

  public toContractsFilter$(routingParams: IContractsRoutingParams): Observable<IContractsFilter> {
    return forkJoin([
      this.getClients$(routingParams.clientIds),
      this.getProducts$(routingParams.productIds),
      this.getOffers$(routingParams.offerIds),
      this.getDistributors$(routingParams.distributorIds),
      this.getEnvironments$(routingParams.environmentIds),
      this.getEstablishments$(routingParams.establishmentIds),
    ]).pipe(
      map(([clients, products, offers, distributors, environments, establishments]) => ({
        ids: !!routingParams.ids ? routingParams.ids.split(',') : null,
        name: routingParams.name,
        states: this.getStates(routingParams.states),
        establishmentHealth: this.getEstablishmentHeath(routingParams.establishmentHealth),
        distributorFilter: this.getDistributorFilter(routingParams.isDirectSales),
        clients,
        products,
        offers,
        distributors,
        environments,
        billingEntities: this.getBillingEntities(routingParams.billingEntityIds),
        establishments,
        createdAt: !!routingParams.createdAt ? new Date(routingParams.createdAt) : null,
        periodOn: {
          startDate: !!routingParams.startAt ? new Date(routingParams.startAt) : null,
          endDate: !!routingParams.endAt ? new Date(routingParams.endAt) : null,
        },
      }),
    ));
  }

  public toContractsRoutingParams(filters: IContractsFilter, columns: ContractAdditionalColumn[]): IContractsRoutingParams {
    return {
      ids: this.getSafeRoutingParams(filters.ids?.join(',')),
      name: this.getSafeRoutingParams(filters.name),
      states: this.getSafeRoutingParams(filters.states?.map(s => s.id).join(',')),
      establishmentHealth: this.getSafeRoutingParams(filters.establishmentHealth?.id?.toString()),
      isDirectSales: this.getDistributorFilterRoutingParams(filters.distributorFilter),
      createdAt: this.getSafeRoutingParams(this.apiV3DateService.toApiV3DateFormat(filters.createdAt)),
      startAt: this.getSafeRoutingParams(this.apiV3DateService.toApiV3DateFormat(filters.periodOn?.startDate)),
      endAt: this.getSafeRoutingParams(this.apiV3DateService.toApiV3DateFormat(filters.periodOn?.endDate)),
      clientIds: this.getSafeRoutingParams(filters.clients?.map(c => c.id).join(',')),
      productIds: this.getSafeRoutingParams(filters.products?.map(p => p.id).join(',')),
      offerIds: this.getSafeRoutingParams(filters.offers?.map(o => o.id).join(',')),
      distributorIds: this.getSafeRoutingParams(filters.distributors?.map(d => d.id).join(',')),
      environmentIds: this.getSafeRoutingParams(filters.environments?.map(e => e.id).join(',')),
      billingEntityIds: this.getSafeRoutingParams(filters.billingEntities?.map(b => b.id).join(',')),
      establishmentIds: this.getSafeRoutingParams(filters.establishments?.map(e => e.id).join(',')),
      columns: this.getSafeRoutingParams(columns.join(',')),
    };
  }

  private getClients$(idsToString: string): Observable<IClient[]> {
    const clientIds = this.convertToNumbers(idsToString);
    if (!clientIds.length) {
      return of([]);
    }

    return this.clientsService.getClientsById$(clientIds).pipe(take(1));
  }

  private getProducts$(idsToString: string): Observable<IProduct[]> {
    const productIds = this.convertToNumbers(idsToString);
    if (!productIds.length) {
      return of([]);
    }

    return this.productsService.getProductsById$(productIds).pipe(take(1));
  }

  private getOffers$(idsToString: string): Observable<IOffer[]> {
    const offerIds = this.convertToNumbers(idsToString);
    if (!offerIds.length) {
      return of([]);
    }

    return this.offersService.getOffersById$(offerIds).pipe(take(1));
  }

  private getDistributors$(ids: string): Observable<IDistributor[]> {
    const distributorIds = !!ids ? ids.split(',') : [];
    if (!distributorIds.length) {
      return of([]);
    }

    return this.distributorsService.getDistributorsById$(distributorIds).pipe(take(1));
  }

  private getEnvironments$(idsToString: string): Observable<IEnvironment[]> {
    const environmentIds = this.convertToNumbers(idsToString);
    if (!environmentIds.length) {
      return of([]);
    }

    return this.environmentsService.getEnvironmentsByIds$(environmentIds).pipe(take(1));
  }

  private getEstablishments$(idsToString: string): Observable<IEstablishment[]> {
    const establishmentIds = this.convertToNumbers(idsToString);
    if (!establishmentIds.length) {
      return of([]);
    }

    return this.establishmentsService.getEstablishmentsById$(establishmentIds).pipe(take(1));
  }

  private getStates(idsToString: string): IContractState[] {
    if (!idsToString) {
      const defaultContractStates = [ContractState.InProgress, ContractState.NotStarted];
      return contractStates.filter(state => defaultContractStates.includes(state.id));
    }

    const stateIds = this.convertToNumbers(idsToString);
    return contractStates.filter(state => stateIds.includes(state.id));
  }

  private getEstablishmentHeath(id: string): IContractEstablishmentHealth {
    if (!id) {
      return null;
    }

    return contractEstablishmentsHealth.find(etsHealth => etsHealth.id === parseInt(id, 10));
  }

  private getDistributorFilter(isDirectSales: string): DistributorFilter {
    if (isDirectSales?.toLowerCase() === 'true') {
      return DistributorFilter.Direct;
    }

    if (isDirectSales?.toLowerCase() === 'false') {
      return DistributorFilter.Indirect;
    }

    return DistributorFilter.All;
  }

  private getDistributorFilterRoutingParams(distributorFilter: DistributorFilter): string {
    if (distributorFilter === DistributorFilter.Direct) {
      return 'true';
    }

    if (distributorFilter === DistributorFilter.Indirect) {
      return 'false';
    }

    return 'true,false';
  }

  private getBillingEntities(ids: string): IBillingEntity[] {
    const billingEntityIds = !!ids ? ids.split(',') : [];
    if (!billingEntityIds.length) {
      return [];
    }

    return billingEntityIds.map(id => getBillingEntity(parseInt(id, 10))).filter(b => !!b);
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
