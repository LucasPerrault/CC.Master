import { Component, Inject, OnInit } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { IOfferPriceList } from '@cc/domain/billing/offers';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { BehaviorSubject } from 'rxjs';
import { finalize } from 'rxjs/operators';

import { PriceListService } from '../../services/price-list.service';

@Component({
  selector: 'cc-price-grid-modal',
  templateUrl: './price-grid-modal.component.html',
})
export class PriceGridModalComponent implements ILuModalContent, OnInit {
  title = this.translatePipe.transform('front_priceGrid_modal_title');

  public offerPriceList: IOfferPriceList;
  public isLoading$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

  public offerId: number;

  constructor(
    @Inject(LU_MODAL_DATA) offerId: number,
    private priceListService: PriceListService,
    private translatePipe: TranslatePipe,
  ) {
    this.offerId = offerId;
  }

  public ngOnInit(): void {
    this.priceListService.getOfferPriceList$(this.offerId)
      .pipe(finalize(() => this.isLoading$.next(false)))
      .subscribe(offerPriceList => this.offerPriceList = offerPriceList);
  }
}

