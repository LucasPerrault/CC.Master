import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { IPriceList } from '@cc/domain/billing/offers';
import { LuModal } from '@lucca-front/ng/modal';
import { Observable } from 'rxjs';

import { IDetailedOffer } from '../../../models/detailed-offer.interface';
import { getCurrency, IOfferCurrency } from '../../../models/offer-currency.interface';
import { OfferRestrictionsService } from '../../../services/offer-restrictions.service';
import { OffersEditionStoreService } from '../offers-edition-store.service';
import { OfferPriceListCreationModalComponent } from './offer-price-list-creation-modal/offer-price-list-creation-modal.component';
import { IOfferPriceListCreationModalData } from './offer-price-list-creation-modal/offer-price-list-creation-modal-data.interface';
import { OfferPriceListDeletionModalComponent } from './offer-price-list-deletion-modal/offer-price-list-deletion-modal.component';
import { IOfferPriceListDeletionModalData } from './offer-price-list-deletion-modal/offer-price-list-deletion-modal-data.interface';
import { OfferPriceListEditionModalComponent } from './offer-price-list-edition-modal/offer-price-list-edition-modal.component';
import { IOfferPriceListEditionModalData } from './offer-price-list-edition-modal/offer-price-list-edition-modal-data.interface';
import { OfferPriceListsTabService, PriceListStatus } from './offer-price-lists-tab.service';

@Component({
  selector: 'cc-offer-price-lists-tab',
  templateUrl: './offer-price-lists-tab.component.html',
  styleUrls: ['./offer-price-lists-tab.component.scss'],
})
export class OfferPriceListsTabComponent implements OnInit {

  public get offer$(): Observable<IDetailedOffer> {
    return this.storeService.offer$;
  }

  public get hasRightToCreateAndEditOffers(): boolean {
    return this.restrictionsService.hasRightToCreateAndEditOffers();
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private luModal: LuModal,
    private datePipe: DatePipe,
    private translatePipe: TranslatePipe,
    private tabService: OfferPriceListsTabService,
    private restrictionsService: OfferRestrictionsService,
    private storeService: OffersEditionStoreService,
  ) { }

  public ngOnInit(): void {
  }

  public getSortedDescLists(lists: IPriceList[]): IPriceList[] {
    return lists.sort((a, b) => new Date(b.startsOn).getTime() - new Date(a.startsOn).getTime());
  }

  public getCurrency(currencyId: number): IOfferCurrency {
    return getCurrency(currencyId);
  }

  public canDelete(priceList: IPriceList): boolean {
    return this.restrictionsService.canDeletePriceList(priceList);
  }

  public openCreationModal(offerId: number, allPriceLists: IPriceList[]): void {
    const allListStartDates = allPriceLists.map(p => new Date(p.startsOn));
    const data: IOfferPriceListCreationModalData = { offerId, allListStartDates };
    this.luModal.open(OfferPriceListCreationModalComponent, data);
  }

  public openEditionModal(offer: IDetailedOffer, priceListToEdit: IPriceList, allPriceLists: IPriceList[]): void {
    const allListStartDates = allPriceLists.map(p => new Date(p.startsOn));
    const data: IOfferPriceListEditionModalData = { offer, priceListToEdit, allListStartDates };
    this.luModal.open(OfferPriceListEditionModalComponent, data);
  }

  public openDeletionModal(offerId: number, priceListId: number, priceListStartsOn: string): void {
    const data: IOfferPriceListDeletionModalData = { offerId, priceListId, priceListStartsOn };
    this.luModal.open(OfferPriceListDeletionModalComponent, data);
  }

  public getPriceListStatus(priceList: IPriceList, allPriceLists: IPriceList[]): string {
    const nextList = this.getNextList(priceList, allPriceLists);
    const start = this.datePipe.transform(new Date(priceList.startsOn), 'MMMM yyyy');

    switch (this.tabService.getStatus(priceList, nextList)) {
      case PriceListStatus.NoDate:
        return this.translatePipe.transform('offers_priceList_status_noDate');
      case PriceListStatus.InProgress:
        return this.translatePipe.transform('offers_priceList_status_inProgress', { start });
      case PriceListStatus.NotStarted:
        return this.translatePipe.transform('offers_priceList_status_notStarted', { start });
      case PriceListStatus.Finished:
        const end = this.datePipe.transform(new Date(nextList.startsOn), 'MMMM yyyy');
        return this.translatePipe.transform('offers_priceList_status_finished', { start, end });
    }
  }

  public trackBy(index: number, priceList: IPriceList): number {
    return priceList.id;
  }

  private getNextList(priceList: IPriceList, allPriceLists: IPriceList[]): IPriceList | null {
    const sortedAscList = allPriceLists.sort((a, b) =>
      new Date(a.startsOn).getTime() - new Date(b.startsOn).getTime());

    const nextListIndex = sortedAscList.findIndex(p => priceList.id === p.id) + 1;
    return sortedAscList[nextListIndex];
  }
}
