import { Component, Inject, OnInit } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { ICurrency, IPriceList, IPriceRow } from '@cc/domain/billing/offers';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { isBefore, isEqual } from 'date-fns';
import { BehaviorSubject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { CurrencyCode, getCurrency } from '../../../offers/models/offer-currency.interface';
import { IPriceGridModalData } from './price-grid-modal-data.interface';
import { PriceGridModalDataService } from './price-grid-modal-data.service';
import { IPriceListOffer } from './price-grid-offer.interface';

@Component({
  selector: 'cc-price-grid-modal',
  templateUrl: './price-grid-modal.component.html',
})
export class PriceGridModalComponent implements ILuModalContent, OnInit {
  title = this.translatePipe.transform('front_priceGrid_modal_title');

  public offer: IPriceListOffer;
  public isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

  public offerId: number;

  constructor(
    @Inject(LU_MODAL_DATA) private data: IPriceGridModalData,
    private priceListService: PriceGridModalDataService,
    private translatePipe: TranslatePipe,
  ) {
    this.offerId = data.offerId;
  }

  public ngOnInit(): void {
    this.priceListService.getOfferPriceList$(this.offerId)
      .pipe(finalize(() => this.isLoading$.next(false)))
      .subscribe(offerPriceList => this.offer = offerPriceList);
  }

  public getPriceList(): IPriceList {
    const sortedDescLists = this.offer.priceLists.sort((a, b) =>
      new Date(b.startsOn).getTime() - new Date(a.startsOn).getTime());

    if (!this.data?.lastCountPeriod) {
      return this.getFirstListWhichStartedBeforeOrEqualContractStartDate(sortedDescLists);
    }


    return this.getFirstListWhichStartedBeforeLastCountPeriod(sortedDescLists);
  }

  public getPriceRows(): IPriceRow[] {
    return this.getPriceList()?.rows ?? [];
  }

  public getCurrency(code: CurrencyCode): ICurrency {
    return getCurrency(code);
  }

  private getFirstListWhichStartedBeforeOrEqualContractStartDate(sortedDesc: IPriceList[]): IPriceList {
    return sortedDesc.find(list =>
      isBefore(new Date(list.startsOn), new Date(this.data.contractStartOn))
      || isEqual(new Date(list.startsOn), new Date(this.data.contractStartOn)),
    );
  }

  private getFirstListWhichStartedBeforeLastCountPeriod(sortedDesc: IPriceList[]): IPriceList {
    return sortedDesc.find(list =>
      isBefore(new Date(list.startsOn), new Date(this.data.lastCountPeriod))
      || isEqual(new Date(list.startsOn), new Date(this.data.lastCountPeriod)),
    );
  }
}

