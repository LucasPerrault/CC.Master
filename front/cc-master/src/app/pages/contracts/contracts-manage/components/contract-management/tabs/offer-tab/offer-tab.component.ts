import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TimelineCountsService } from '@cc/domain/billing/counts/timeline-counts-service';
import { forkJoin, Observable, ReplaySubject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { IOfferContract } from './models/offer-contract.interface';
import { ISimilarOfferContext } from './models/similar-offer-context.interface';
import { OfferTabDataService } from './services/offer-tab-data.service';

@Component({
  selector: 'cc-offer-tab',
  templateUrl: './offer-tab.component.html',
  styleUrls: ['./offer-tab.component.scss'],
})
export class OfferTabComponent implements OnInit {
  public isLoading$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  public formControl: FormControl = new FormControl();
  public context$: ReplaySubject<ISimilarOfferContext> = new ReplaySubject<ISimilarOfferContext>(1);

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  constructor(private activatedRoute: ActivatedRoute, private dataService: OfferTabDataService) { }

  public ngOnInit(): void {
    this.isLoading$.next(true);

    const requests$ = [
      this.dataService.getContract$(this.contractId),
      this.getLastCountPeriod$(this.contractId),
    ];

    forkJoin(requests$)
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(([contract, lastCountPeriod]: [IOfferContract, Date]) => this.setSimilarOfferContext(contract, lastCountPeriod));
  }

  private getLastCountPeriod$(contractId: number): Observable<Date> {
    return this.dataService.getRealCounts$(contractId)
      .pipe(map(counts => TimelineCountsService.getLastCountPeriod(counts)));
  }

  private setSimilarOfferContext(contract: IOfferContract, lastCountPeriod: Date): void {
    const offerId = contract.commercialOfferId;
    const maxPeriod = lastCountPeriod ?? new Date(contract.theoreticalStartOn);

    this.context$.next({ offerId, maxPeriod });
  }
}
