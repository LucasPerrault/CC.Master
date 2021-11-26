import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { TimelineCountsService } from '@cc/domain/billing/counts/timeline-counts-service';
import { forkJoin, Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { ContractManagementService } from '../../contract-management.service';
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

  public editButtonClass$: Subject<string> = new Subject<string>();

  private readonly contractId: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private dataService: OfferTabDataService,
    private pageService: ContractManagementService,
  ) {
    this.contractId = parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

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

  public edit() {
    const offerId = this.formControl.value?.id;
    this.dataService.editContract$(this.contractId, offerId)
      .pipe(take(1), toSubmissionState(), map(state => getButtonState(state)))
      .subscribe(this.editButtonClass$);
  }

  public close(): void {
    this.pageService.close();
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
