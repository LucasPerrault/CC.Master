import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslatePipe } from '@cc/aspects/translate';
import { IPriceList } from '@cc/domain/billing/offers';
import { LuModal } from '@lucca-front/ng/modal';
import { ReplaySubject } from 'rxjs';
import { finalize, take } from 'rxjs/operators';

import { IDetailedOffer } from '../../../models/detailed-offer.interface';
import { OffersDataService } from '../../../services/offers-data.service';
import { OfferPriceListCreationModalComponent } from './offer-price-list-creation-modal/offer-price-list-creation-modal.component';
import { OfferPriceListsTabService, PriceListStatus } from './offer-price-lists-tab.service';

@Component({
  selector: 'cc-offer-price-lists-tab',
  templateUrl: './offer-price-lists-tab.component.html',
  styleUrls: ['./offer-price-lists-tab.component.scss'],
})
export class OfferPriceListsTabComponent implements OnInit {
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public offer$: ReplaySubject<IDetailedOffer> = new ReplaySubject<IDetailedOffer>(1);

  private get offerId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  constructor(
    private activatedRoute: ActivatedRoute,
    private dataService: OffersDataService,
    private luModal: LuModal,
    private datePipe: DatePipe,
    private translatePipe: TranslatePipe,
    private tabService: OfferPriceListsTabService,
  ) { }

  public ngOnInit(): void {
    this.reset();
  }

  public reset(): void {
    this.isLoading$.next(true);

    this.dataService.getById$(this.offerId)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(priceLists => this.offer$.next(priceLists));
  }

  public openCreationModal(offer: IDetailedOffer): void {
    const dialogRef = this.luModal.open(OfferPriceListCreationModalComponent, offer);
    dialogRef.onClose.pipe(take(1)).subscribe(() => this.reset());
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
