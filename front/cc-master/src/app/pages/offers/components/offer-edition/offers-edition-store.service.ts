import { Injectable } from '@angular/core';
import { SubmissionState, toSubmissionState } from '@cc/common/forms';
import { BehaviorSubject, combineLatest, forkJoin, Observable, of, ReplaySubject } from 'rxjs';
import { finalize, map, switchMapTo, take, tap } from 'rxjs/operators';

import { IDetailedOffer } from '../../models/detailed-offer.interface';
import { IOfferValidationContext } from '../../models/offer-validation-context.interface';
import { IPriceListForm } from '../../models/price-list-form.interface';
import { OfferListService } from '../../services/offer-list.service';
import { OfferValidationContextDataService } from '../../services/offer-validation-context-data.service';
import { OffersDataService } from '../../services/offers-data.service';
import { PriceListsDataService } from '../../services/price-lists-data.service';
import { IOfferEditionForm } from './offer-edition-tab/offer-edition-form/offer-edition-form.interface';

@Injectable()
export class OffersEditionStoreService {

  public get offer$(): Observable<IDetailedOffer> {
    return this.offer.asObservable();
  }

  public get validationContext$(): Observable<IOfferValidationContext> {
    return this.validationContext.asObservable();
  }

  public get state$(): Observable<SubmissionState> {
    return this.state.asObservable();
  }

  public get loading$(): Observable<boolean> {
    return this.state$.pipe(map(state => state === SubmissionState.Load));
  }

  private offer: ReplaySubject<IDetailedOffer> = new ReplaySubject<IDetailedOffer>(1);
  private validationContext: ReplaySubject<IOfferValidationContext> = new ReplaySubject<IOfferValidationContext>(1);

  private state: BehaviorSubject<SubmissionState> = new BehaviorSubject<SubmissionState>(SubmissionState.Idle);

  constructor(
    private offersDataService: OffersDataService,
    private listsDataService: PriceListsDataService,
    private contextDataService: OfferValidationContextDataService,
    private offerListService: OfferListService,
  ) {
  }

  public init(offerId: number): void {
    this.refresh(offerId);
  }

  public editOfferAndPriceList$(offerId: number, priceListId: number, form: IOfferEditionForm): Observable<void> {
    const requests$ = [
      this.offersDataService.edit$(offerId, form),
      this.listsDataService.edit$(offerId, priceListId, form.priceList),
    ];

    return forkJoin(requests$).pipe(
      switchMapTo(of<void>()),
      finalize(() => {
        this.offerListService.refresh();
        this.refresh(offerId);
      }),
    );
  }

  public createPriceList$(offerId: number, form: IPriceListForm): Observable<void> {
    return this.listsDataService.create$(offerId, form)
      .pipe(finalize(() => this.refresh(offerId)));
  }

  public editPriceList$(offerId: number, priceListId: number, form: IPriceListForm): Observable<void> {
    return this.listsDataService.edit$(offerId, priceListId, form)
      .pipe(finalize(() => this.refresh(offerId)));
  }

  public deletePriceList$(offerId: number, priceListId: number): Observable<void> {
    return this.listsDataService.delete$(offerId, priceListId)
      .pipe(finalize(() => this.refresh(offerId)));
  }

  private refresh(offerId: number): void {
    combineLatest([
      this.offersDataService.getById$(offerId),
      this.contextDataService.getRealCountNumber$(offerId),
    ])
      .pipe(
        take(1),
        tap(([offer, realCountNumber]) => this.set(offer, realCountNumber)),
        toSubmissionState(),
      )
      .subscribe(state => this.state.next(state));
  }

  private set(offer: IDetailedOffer, realCountNumber: number): void {
    this.offer.next(offer);
    this.validationContext.next({ offer, realCountNumber });
  }
}
