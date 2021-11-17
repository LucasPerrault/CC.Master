import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { INavigationTab } from '@cc/common/navigation';
import { IContract } from '@cc/domain/billing/contracts';
import { from, ReplaySubject } from 'rxjs';
import { catchError, take } from 'rxjs/operators';

import { OffersDataService } from '../../services/offers-data.service';
import { navigationTabs, OfferEditionNavigationPath } from './offer-edition-navigation-tabs.const';

@Component({
  selector: 'cc-offer-edition',
  templateUrl: './offer-edition.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./offer-edition.component.scss'],
})
export class OfferEditionComponent implements OnInit {
  public offerName$: ReplaySubject<string> = new ReplaySubject<string>(1);

  public get tabs(): INavigationTab[] {
    return navigationTabs;
  }

  private get offerId(): number {
    return parseInt(this.activatedRoute.snapshot.paramMap.get('id'), 10);
  }

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private offersDataService: OffersDataService,
  ) { }

  public ngOnInit(): void {
    this.offersDataService.getName$(this.offerId)
      .pipe(take(1), catchError(() => from(this.redirectToNotFoundPage())))
      .subscribe(this.offerName$);
  }

  private redirectToNotFoundPage(): Promise<IContract> {
    const route = [OfferEditionNavigationPath.NotFound];
    return this.router
      .navigate(route, { relativeTo: this.activatedRoute })
      .then(() => null);
  }
}
