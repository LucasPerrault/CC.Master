import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { getButtonState, toSubmissionState } from '@cc/common/forms';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { finalize, map, take, takeUntil } from 'rxjs/operators';

import { establishmentDocUrl } from './constants/establishment-doc-url.const';
import { EstablishmentType } from './constants/establishment-type.enum';
import { IContractCount } from './models/contract-count.interface';
import { IEstablishmentContract } from './models/establishment-contract.interface';
import { IEstablishmentsWithAttachmentsByType } from './models/establishments-by-type.interface';
import { EstablishmentContractDataService } from './services/establishment-contract-data.service';
import { EstablishmentListActionsService } from './services/establishment-list-actions.service';
import { EstablishmentTypeService } from './services/establishment-type.service';
import { EstablishmentsDataService } from './services/establishments-data.service';
import { EstablishmentsWithAttachmentsService } from './services/establishments-with-attachments.service';

@Component({
  selector: 'cc-establishment-tab',
  templateUrl: './establishment-tab.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./establishment-tab.component.scss'],
})
export class EstablishmentTabComponent implements OnInit, OnDestroy {

  public get hasEnvironment$(): Observable<boolean> {
    return this.contract$.pipe(map(contract => !!contract.environmentId));
  }

  public get contractId(): number {
    return parseInt(this.activatedRoute.parent.snapshot.paramMap.get('id'), 10);
  }

  public establishmentType = EstablishmentType;

  public establishments$: ReplaySubject<IEstablishmentsWithAttachmentsByType> = new ReplaySubject<IEstablishmentsWithAttachmentsByType>(1);
  public contract$: ReplaySubject<IEstablishmentContract> = new ReplaySubject<IEstablishmentContract>(1);
  public realCounts$: ReplaySubject<IContractCount[]> = new ReplaySubject<IContractCount[]>(1);

  public isLoading$: ReplaySubject<boolean> = new ReplaySubject(1);
  public synchronizeButtonClass$: ReplaySubject<string> = new ReplaySubject(1);

  private destroy$: Subject<void> = new Subject();

  constructor(
    private activatedRoute: ActivatedRoute,
    private establishmentsListService: EstablishmentsWithAttachmentsService,
    private contractService: EstablishmentContractDataService,
    private establishmentsService: EstablishmentsDataService,
    private actionsService: EstablishmentListActionsService,
    private typeService: EstablishmentTypeService,
  ) {
  }

  public ngOnInit(): void {
    this.refresh();

    this.actionsService.refreshList$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.refresh());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public openEstablishmentsDoc(): void {
    window.open(establishmentDocUrl);
  }

  public synchronize(environmentId: number): void {
    this.establishmentsService.synchronize(environmentId)
      .pipe(
        toSubmissionState(),
        map(state => getButtonState(state)),
        finalize(() => this.refresh()),
      )
      .subscribe(buttonClass => this.synchronizeButtonClass$.next(buttonClass));
  }

  private refresh(): void {
    this.isLoading$.next(true);

    this.contractService.getContract$(this.contractId)
      .pipe(take(1))
      .subscribe(contract => this.set(contract));
  }

  private set(contract: IEstablishmentContract): void {
    combineLatest([
      this.contractService.getRealCounts$(this.contractId),
      this.getEstablishmentsByType$(contract),
    ])
      .pipe(take(1), finalize(() => this.isLoading$.next(false)))
      .subscribe(([realCounts, establishments]) => {
        this.contract$.next(contract);
        this.realCounts$.next(realCounts);
        this.establishments$.next(establishments);
      });
  }

  private getEstablishmentsByType$(contract: IEstablishmentContract): Observable<IEstablishmentsWithAttachmentsByType> {
    return this.establishmentsListService.getEstablishments$(contract)
      .pipe(map(ets => this.typeService.getEstablishmentsByType(ets, contract)));
  }
}
