import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { TimelineCountsService } from '@cc/domain/billing/counts/timeline-counts-service';
import { BehaviorSubject, forkJoin, Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take } from 'rxjs/operators';

import { ContractManagementService } from '../../contract-management.service';
import { IOfferContract } from './models/offer-contract.interface';
import { ISimilarOfferContext } from './models/similar-offer-context.interface';
import { OfferTabDataService } from './services/offer-tab-data.service';
import { LuModal } from '@lucca-front/ng/modal';
import { PriceGridModalComponent } from '../../../../../common/price-grid-modal/price-grid-modal.component';
import { IPriceGridModalData } from '../../../../../common/price-grid-modal/price-grid-modal-data.interface';

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

  private contract$: BehaviorSubject<IOfferContract> = new BehaviorSubject<IOfferContract>(null);
  private lastCountPeriod$: BehaviorSubject<Date> = new BehaviorSubject<Date>(null);
  private readonly contractId: number;

  constructor(
    private activatedRoute: ActivatedRoute,
    private dataService: OfferTabDataService,
    private pageService: ContractManagementService,
    private luModal: LuModal,
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
      .subscribe(([contract, lastCountPeriod]: [IOfferContract, Date]) => this.set(contract, lastCountPeriod));
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

  public openPriceGridModal(): void {
    const contract = this.contract$.value;
    const offerId = this.formControl.value?.id;
    const contractStartOn = contract.theoreticalStartOn;
    const lastCountPeriod = this.lastCountPeriod$.value;

    const data: IPriceGridModalData = { offerId, contractStartOn, lastCountPeriod };
    this.luModal.open(PriceGridModalComponent, data);
  }

  private getLastCountPeriod$(contractId: number): Observable<Date> {
    return this.dataService.getRealCounts$(contractId)
      .pipe(map(counts => TimelineCountsService.getLastCountPeriod(counts)));
  }

  private set(contract: IOfferContract, lastCountPeriod: Date): void {
    this.contract$.next(contract);
    this.lastCountPeriod$.next(lastCountPeriod);

    const offerId = contract.commercialOfferId;
    const maxPeriod = lastCountPeriod ?? new Date(contract.theoreticalStartOn);
    this.context$.next({ offerId, maxPeriod });
  }
}
