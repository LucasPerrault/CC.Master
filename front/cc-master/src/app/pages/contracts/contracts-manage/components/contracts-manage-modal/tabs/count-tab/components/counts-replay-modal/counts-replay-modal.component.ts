import { ChangeDetectionStrategy, Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IDateRange } from '@cc/common/date';
import { EndDateGranularityPolicy,IDateRangeConfiguration } from '@cc/common/forms/select/date-range-select';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { endOfMonth, isAfter, isBefore, isEqual, startOfMonth } from 'date-fns';
import { combineLatest, Observable, ReplaySubject, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { ICountListEntry } from '../../models/count-list-entry.interface';
import { ICountsReplayModalData } from '../../models/counts-replay-modal-data.interface';
import { CountContractsDataService } from '../../services/count-contracts-data.service';

@Component({
  selector: 'cc-counts-replay-modal',
  templateUrl: './counts-replay-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CountsReplayModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public formControl: FormControl = new FormControl();

  title = this.translatePipe.transform('front_contractPage_counts_replay_modal_title');
  submitLabel = this.translatePipe.transform('front_contractPage_counts_replay_modal_submitLabel');
  submitDisabled = true;

  public dateRangeConfiguration: IDateRangeConfiguration = {
    granularity: ELuDateGranularity.month,
    periodCoverStrategy: EndDateGranularityPolicy.End,
    startDateConfiguration: {
      class: 'mod-inline mod-long palette-grey',
      min: this.modalData.min,
      max: this.modalData.max,
    },
    endDateConfiguration: {
      class: 'mod-inline mod-long palette-grey',
      min: this.modalData.min,
      max: this.modalData.max,
    },
  };

  public hasSelectedPeriodValid$: ReplaySubject<boolean> = new ReplaySubject<boolean>(1);

  private destroy$: Subject<void> = new Subject();

  constructor(
    @Inject(LU_MODAL_DATA) private modalData: ICountsReplayModalData,
    private translatePipe: TranslatePipe,
    private countContractsService: CountContractsDataService,
  ) { }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(
        map((range: IDateRange) => {
          const entries = this.getCountEntriesBetween(range?.startDate, range?.endDate);
          return this.canDeleteAllCounts(entries);
        }))
      .subscribe(isValid => this.hasSelectedPeriodValid$.next(isValid));

    combineLatest([this.formControl.valueChanges, this.hasSelectedPeriodValid$])
      .pipe(takeUntil(this.destroy$))
      .subscribe(([, hasSelectedPeriodValid]) =>
        this.submitDisabled = this.formControl.invalid || !hasSelectedPeriodValid,
      );
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    const from = startOfMonth(new Date(this.formControl.value?.startDate));
    const to = endOfMonth(new Date(this.formControl.value?.endDate));

    return this.countContractsService.charge$(this.modalData.contractId, from, to);
  }

  private getCountEntriesBetween(from: Date, to: Date): ICountListEntry[] {
    if (!from || !to) {
      return [];
    }

    return this.modalData.entries.filter(e =>
      (isAfter(e.month, from) || isEqual(e.month, from))
      && (isBefore(e.month, to) || isEqual(e.month, to)),
    );
  }

  private canDeleteAllCounts(entries: ICountListEntry[]): boolean {
    return entries.every(e => !e.count || e.count.canDelete);
  }

}
