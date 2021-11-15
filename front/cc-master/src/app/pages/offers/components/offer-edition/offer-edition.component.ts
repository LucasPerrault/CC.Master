import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ReplaySubject } from 'rxjs';
import { take } from 'rxjs/operators';

import { OffersDataService } from '../../services/offers-data.service';

@Component({
  selector: 'cc-offer-edition',
  templateUrl: './offer-edition.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./offer-edition.component.scss'],
})
export class OfferEditionComponent implements OnInit {
  public offerName$: ReplaySubject<string> = new ReplaySubject<string>(1);

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
      .pipe(take(1))
      .subscribe(this.offerName$);
  }
}
