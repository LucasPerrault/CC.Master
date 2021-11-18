import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { SubmissionState } from '@cc/common/forms';
import { INavigationTab } from '@cc/common/navigation';
import { IContract } from '@cc/domain/billing/contracts';
import { Observable, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { navigationTabs, OfferEditionNavigationPath } from './offer-edition-navigation-tabs.const';
import { OffersEditionStoreService } from './offers-edition-store.service';

@Component({
  selector: 'cc-offer-edition',
  templateUrl: './offer-edition.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./offer-edition.component.scss'],
})
export class OfferEditionComponent implements OnInit, OnDestroy {

  public get offerName$(): Observable<string> {
    return this.storeService.offer$.pipe(map(offer => offer.name));
  }

  public get loading$(): Observable<boolean> {
    return this.storeService.loading$;
  }

  public get tabs(): INavigationTab[] {
    return navigationTabs;
  }

  private destroy$: Subject<void> = new Subject();

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private storeService: OffersEditionStoreService,
  ) { }

  public ngOnInit(): void {
    this.storeService.state$
      .pipe(takeUntil(this.destroy$), filter(state => state === SubmissionState.Error))
      .subscribe(() => this.redirectToNotFoundPage());

    const offerId = parseInt(this.activatedRoute.snapshot.paramMap.get('id'), 10);
    this.storeService.init(offerId);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private redirectToNotFoundPage(): Promise<IContract> {
    const route = [OfferEditionNavigationPath.NotFound];
    return this.router
      .navigate(route, { relativeTo: this.activatedRoute })
      .then(() => null);
  }
}
