import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { NavigationPath } from '@cc/common/navigation';
import { ReplaySubject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { OfferListService } from '../../services/offer-list.service';
import { OffersDataService } from '../../services/offers-data.service';
import { IOfferForm } from '../offer-form/offer-form.interface';

@Component({
  selector: 'cc-offer-creation',
  templateUrl: './offer-creation.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./offer-creation.component.scss'],
})
export class OfferCreationComponent {
  public formControl: FormControl = new FormControl();
  public creationButtonState$: ReplaySubject<string> = new ReplaySubject<string>(1);

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private offersDataService: OffersDataService,
    private offerListService: OfferListService,
  ) { }

  public create(): void {
    const form: IOfferForm = this.formControl.value;

    this.offersDataService.create$(form)
      .pipe(
        take(1),
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => {
          this.offerListService.refresh();
          this.redirectToOffers();
        }),
      )
      .subscribe(state => this.creationButtonState$.next(state));
  }

  public cancel(): void {
    this.formControl.reset();
    this.redirectToOffers();
  }

  private redirectToOffers(): void {
    this.router.navigate([NavigationPath.Offers]);
  }
}
