import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { NavigationPath } from '@cc/common/navigation';
import { ReplaySubject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { getBillingMode } from '../../enums/billing-mode.enum';
import { getBillingUnit } from '../../enums/billing-unit.enum';
import { IDetailedOffer } from '../../models/detailed-offer.interface';
import { OfferListService } from '../../services/offer-list.service';
import { OffersDataService } from '../../services/offers-data.service';
import { IOfferForm } from '../offer-form/offer-form.interface';

@Component({
  selector: 'cc-offer-edition',
  templateUrl: './offer-edition.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./offer-edition.component.scss'],
})
export class OfferEditionComponent implements OnInit {
  public offer$: ReplaySubject<IDetailedOffer> = new ReplaySubject<IDetailedOffer>(1);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public formControl: FormControl = new FormControl();
  public editionButtonState$: ReplaySubject<string> = new ReplaySubject<string>(1);

  private get offerId(): number {
    return parseInt(this.activatedRoute.snapshot.paramMap.get('id'), 10);
  }

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private offersDataService: OffersDataService,
    private offerListService: OfferListService,
  ) { }

  public ngOnInit(): void {
    this.isLoading$.next(true);

    this.offersDataService.getById$(this.offerId)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(offer => {
        this.offer$.next(offer);
        this.formControl.patchValue(this.toOfferForm(offer));
      });
  }

  public edit(): void {
    const form: IOfferForm = this.formControl.value;

    this.offersDataService.edit$(this.offerId, form)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.offerListService.refresh();
          this.redirectToOffers();
        }),
      )
      .subscribe(state => this.editionButtonState$.next(state));
  }

  public cancel(): void {
    this.formControl.reset();
    this.redirectToOffers();
  }

  private redirectToOffers(): void {
    this.router.navigate([NavigationPath.Offers]);
  }

  private toOfferForm(offer: IDetailedOffer): IOfferForm {
    return {
      name: offer.name,
      product: offer.product,
      billingMode: getBillingMode(offer.billingMode),
      currency: offer.currency,
      sageBusiness: offer.sageBusiness,
      tag: offer.tag,
      billingUnit: getBillingUnit(offer.unit),
      pricingMethod: offer.pricingMethod,
      forecastMethod: offer.forecastMethod,
      priceLists: offer.priceLists,
    };
  }

}
