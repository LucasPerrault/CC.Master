import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LuModal } from '@lucca-front/ng/modal';
import { ReplaySubject } from 'rxjs';
import { finalize, take } from 'rxjs/operators';

import { IDetailedOffer } from '../../../models/detailed-offer.interface';
import { OffersDataService } from '../../../services/offers-data.service';
import { OfferPriceListCreationModalComponent } from './offer-price-list-creation-modal/offer-price-list-creation-modal.component';

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
}
