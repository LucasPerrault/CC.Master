import { HttpParams } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { defaultPagingParams, IPaginatedResult, PaginatedList, PaginatedListState, PagingService } from '@cc/common/paging';
import { ApiStandard } from '@cc/common/queries';
import { ISortParams, SortOrder } from '@cc/common/sort';
import { BehaviorSubject, combineLatest, Observable, Subject } from 'rxjs';
import { debounceTime, map, switchMap, takeUntil } from 'rxjs/operators';

import { OfferSortParamKey } from './enums/offer-sort-param-key.enum';
import { IDetailedOffer, IDetailedOfferWithoutUsage } from './models/detailed-offer.interface';
import { IOfferUsage } from './models/offer-usage.interface';
import { OfferListService } from './services/offer-list.service';
import { OfferRestrictionsService } from './services/offer-restrictions.service';
import { OfferUsageStoreService } from './services/offer-usage-store.service';
import { OffersApiMappingService } from './services/offers-api-mapping.service';
import { OffersDataService } from './services/offers-data.service';

// It is defined in the offer model in the back project.
// It is used for the default selection.
const offerPrincipalTag = 'Catalogues';

@Component({
  selector: 'cc-offers',
  templateUrl: './offers.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OffersComponent implements OnInit {

  public get offers$(): Observable<IDetailedOffer[]> {
    return this.paginatedOffers.items$;
  }

  public get totalCounts$(): Observable<number> {
    return this.paginatedOffers.totalCount$;
  }

  public get isLoading$(): Observable<boolean> {
    return this.paginatedOffers.state$.pipe(
      map(state => state === PaginatedListState.Update),
    );
  }

  public get canCreateOffer(): boolean {
    return this.restrictionsService.hasRightToCreateOffers();
  }

  public filters: FormControl = new FormControl();
  public sortParams$: BehaviorSubject<ISortParams> = new BehaviorSubject<ISortParams>(null);

  public paginatedOffers: PaginatedList<IDetailedOffer>;

  private destroy$: Subject<void> = new Subject();

  constructor(
    private restrictionsService: OfferRestrictionsService,
    private apiMappingService: OffersApiMappingService,
    private offersDataService: OffersDataService,
    private usageStoreService: OfferUsageStoreService,
    private offerListService: OfferListService,
    private pagingService: PagingService,
  ) {
  }

  public ngOnInit(): void {
    combineLatest([this.filters.valueChanges, this.sortParams$])
      .pipe(
        takeUntil(this.destroy$),
        debounceTime(300),
        map(([filters, sort]) => ({ filters, sort })),
        map(attributes => this.apiMappingService.toHttpParams(attributes)),
      )
      .subscribe(httpParams => this.paginatedOffers.updateHttpParams(httpParams));

    this.initDefaultFiltersAndSort();

    this.paginatedOffers = this.pagingService.paginate<IDetailedOffer>(
      (httpParams) => this.getPaginatedOffers$(httpParams),
      { page: defaultPagingParams.page, limit: defaultPagingParams.limit },
      ApiStandard.V4,
    );

    this.offerListService.refresh$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.refresh());
  }

  public nextPage(): void {
    this.paginatedOffers.nextPage();
  }

  public updateSort(sortParams: ISortParams) {
    this.sortParams$.next(sortParams);
  }

  private refresh(): void {
    const filters = this.filters.value;
    const sort = this.sortParams$.value;
    const httpParams = this.apiMappingService.toHttpParams({ filters, sort });
    this.paginatedOffers.updateHttpParams(httpParams);
  }

  private getPaginatedOffers$(httpParams: HttpParams): Observable<IPaginatedResult<IDetailedOffer>> {
    return this.offersDataService.getOffersWithoutUsage$(httpParams).pipe(
      switchMap(response => this.getDetailedOffers$(response.items, response.count)),
    );
  }

  private getDetailedOffers$(offers: IDetailedOfferWithoutUsage[], count: number): Observable<IPaginatedResult<IDetailedOffer>> {
    const offerIds = offers.map(o => o.id) ?? [];
    return this.usageStoreService.getUsages$(offerIds).pipe(
      map(usages => ({ items: this.toDetailedOffers(offers, usages), totalCount: count })));
  }

  private toDetailedOffers(offers: IDetailedOfferWithoutUsage[], usages: IOfferUsage[]): IDetailedOffer[] {
    return offers.map(offer => ({ ...offer, usage: usages.find(u => u.offerId === offer.id) }));
  }

  private initDefaultFiltersAndSort(): void {
    this.sortParams$.next({ field: OfferSortParamKey.Name, order: SortOrder.Asc });
    this.filters.patchValue({ tag: offerPrincipalTag });
  }
}
