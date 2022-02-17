import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { NavigationPath } from '@cc/common/navigation';
import { IPriceList } from '@cc/domain/billing/offers';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import { map, take } from 'rxjs/operators';

import { getBillingMode } from '../../../enums/billing-mode.enum';
import { getBillingUnit } from '../../../enums/billing-unit.enum';
import { IDetailedOffer } from '../../../models/detailed-offer.interface';
import { getCurrency } from '../../../models/offer-currency.interface';
import { IPriceListForm } from '../../../models/price-list-form.interface';
import { OfferRestrictionsService } from '../../../services/offer-restrictions.service';
import { PriceListsTimelineService } from '../../../services/price-lists-timeline.service';
import { OffersEditionStoreService } from '../offers-edition-store.service';
import { IOfferEditionForm } from './offer-edition-form/offer-edition-form.interface';

@Component({
  selector: 'cc-offer-edition-tab',
  templateUrl: './offer-edition-tab.component.html',
  styleUrls: ['./offer-edition-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferEditionTabComponent implements OnInit {

  public offerToEdit$: BehaviorSubject<IDetailedOffer> = new BehaviorSubject<IDetailedOffer>(null);
  public priceListToEdit$: BehaviorSubject<IPriceList> = new BehaviorSubject<IPriceList>(null);

  public formControl: FormControl = new FormControl();
  public editionButtonState$: ReplaySubject<string> = new ReplaySubject<string>(1);

  public get hasRightToCreateAndEditOffers(): boolean {
    return this.restrictionsService.hasRightToCreateAndEditOffers();
  }

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private storeService: OffersEditionStoreService,
    private restrictionsService: OfferRestrictionsService,
  ) { }

  public ngOnInit(): void {
    this.storeService.offer$
      .pipe(take(1))
      .subscribe(offer => {
        this.formControl.patchValue(this.toOfferForm(offer));
        this.priceListToEdit$.next(PriceListsTimelineService.getCurrent(offer.priceLists));
        this.offerToEdit$.next(offer);
      });
  }

  public edit(): void {
    const form: IOfferEditionForm = this.formControl.value;

    this.storeService.editOffer$(this.offerToEdit$.value, this.priceListToEdit$.value, form)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
      )
      .subscribe(state => this.editionButtonState$.next(state));
  }

  public cancel(): void {
    this.formControl.reset();
    this.redirectToOffers();
  }

  private redirectToOffers(): void {
    this.router.navigate([NavigationPath.Offers], { queryParamsHandling: 'preserve' });
  }

  private toOfferForm(offer: IDetailedOffer): IOfferEditionForm {
    return {
      name: offer.name,
      product: offer.product,
      billingMode: getBillingMode(offer.billingMode),
      currency: getCurrency(offer.currencyId),
      tag: offer.tag,
      billingUnit: getBillingUnit(offer.unit),
      pricingMethod: offer.pricingMethod,
      forecastMethod: offer.forecastMethod,
      priceList: this.toPriceListForm(PriceListsTimelineService.getCurrent(offer.priceLists)),
    };
  }

  private toPriceListForm(priceList: IPriceList): IPriceListForm {
    return {
      startsOn: !!priceList?.startsOn ? new Date(priceList.startsOn) : null,
      rows: priceList?.rows ?? [],
    };
  }
}
