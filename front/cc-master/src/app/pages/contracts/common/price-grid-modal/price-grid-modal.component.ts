import { Component, Inject, OnInit } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { IPriceListOffer, IPriceRow } from '@cc/domain/billing/offers';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { BehaviorSubject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { PriceGridModalDataService } from './price-grid-modal-data.service';

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
    @Inject(LU_MODAL_DATA) offerId: number,
    private priceListService: PriceGridModalDataService,
    private translatePipe: TranslatePipe,
  ) {
    this.offerId = offerId;
  }

  public ngOnInit(): void {
    this.priceListService.getOfferPriceList$(this.offerId)
      .pipe(finalize(() => this.isLoading$.next(false)))
      .subscribe(offerPriceList => this.offer = offerPriceList);
  }

  public getPriceRows(): IPriceRow[] {
    return this.offer.priceLists[0]?.rows ?? [];
  }
}

