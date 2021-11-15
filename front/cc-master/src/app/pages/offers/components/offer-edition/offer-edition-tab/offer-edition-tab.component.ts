import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { NavigationPath } from '@cc/common/navigation';
import { IPriceList } from '@cc/domain/billing/offers';
import { BehaviorSubject, combineLatest, ReplaySubject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { getBillingMode } from '../../../enums/billing-mode.enum';
import { getBillingUnit } from '../../../enums/billing-unit.enum';
import { IDetailedOffer } from '../../../models/detailed-offer.interface';
import { IPriceListForm } from '../../../models/price-list-form.interface';
import { OfferListService } from '../../../services/offer-list.service';
import { OfferPriceListService } from '../../../services/offer-price-list.service';
import { OffersDataService } from '../../../services/offers-data.service';
import { IOfferEditionForm } from './offer-edition-form/offer-edition-form.interface';
import { IOfferEditionValidationContext } from '../offer-edition-validation-context.interface';
import { OfferEditionValidationContextService } from '../offer-edition-validation-context.service';

@Component({
  selector: 'cc-offer-edition-tab',
  templateUrl: './offer-edition-tab.component.html',
  styleUrls: ['./offer-edition-tab.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OfferEditionTabComponent implements OnInit {

  public priceListId$: BehaviorSubject<number> = new BehaviorSubject<number>(null);
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);
  public validationContext$ = new ReplaySubject<IOfferEditionValidationContext>(1);

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
    private contextValidationService: OfferEditionValidationContextService,
  ) { }

  public ngOnInit(): void {
    this.reset();
  }

  public edit(): void {
    const form: IOfferEditionForm = this.formControl.value;

    this.offersDataService.edit$(this.offerId, this.priceListId$.value, form)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.offerListService.refresh();
          this.reset();
        }),
      )
      .subscribe(state => this.editionButtonState$.next(state));
  }

  public cancel(): void {
    this.formControl.reset();
    this.redirectToOffers();
  }

  private reset(): void {
    this.isLoading$.next(true);

    combineLatest([
      this.offersDataService.getById$(this.offerId),
      this.contextValidationService.getRealCountNumber$(this.offerId),
    ])
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(([offer, realCountNumber]) => {
        this.priceListId$.next(OfferPriceListService.getCurrent(offer.priceLists)?.id);
        this.formControl.patchValue(this.toOfferForm(offer));
        this.validationContext$.next({ offer, realCountNumber });
      });
  }

  private redirectToOffers(): void {
    this.router.navigate([NavigationPath.Offers]);
  }

  private toOfferForm(offer: IDetailedOffer): IOfferEditionForm {
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
      priceList: this.toPriceListForm(OfferPriceListService.getCurrent(offer.priceLists)),
    };
  }

  private toPriceListForm(priceList: IPriceList): IPriceListForm {
    return {
      startsOn: !!priceList.startsOn ? new Date(priceList.startsOn) : null,
      rows: priceList.rows,
    };
  }
}
