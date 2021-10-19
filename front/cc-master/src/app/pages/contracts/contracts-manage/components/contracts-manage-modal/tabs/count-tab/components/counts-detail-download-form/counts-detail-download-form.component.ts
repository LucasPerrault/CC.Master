import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { IDateRange } from '@cc/common/date';
import { IDateRangeConfiguration, EndDateGranularityPolicy } from '@cc/common/forms/select/date-range-select';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { endOfMonth, startOfMonth, subMonths } from 'date-fns';
import { combineLatest, Observable, ReplaySubject } from 'rxjs';
import { map, shareReplay, take } from 'rxjs/operators';

import { ICountContract } from '../../models/count-contract.interface';
import { IDetailedCount } from '../../models/detailed-count.interface';

enum DownloadFormKey {
  DateRange = 'period',
}

@Component({
  selector: 'cc-counts-detail-download-form',
  templateUrl: './counts-detail-download-form.component.html',
})
export class CountsDetailDownloadFormComponent implements OnInit {
  @Input() public downloadButtonState: string;
  @Input() public set firstCountWithDetails(c: IDetailedCount) {
    this.countPeriod$.next(!!c?.countPeriod ? new Date(c.countPeriod) : null);
  }
  @Input() public set contract(c: ICountContract) {
    this.closeOn$.next(!!c?.closeOn ? new Date(c.closeOn) : null);
  }

  @Output() public downloadCountsDetail: EventEmitter<IDateRange> = new EventEmitter<IDateRange>();
  @Output() public cancel: EventEmitter<void> = new EventEmitter<void>();

  public formGroup: FormGroup;
  public formGroupKey = DownloadFormKey;

  public dateRangeConfig$: Observable<IDateRangeConfiguration>;
  public countPeriod$: ReplaySubject<Date> = new ReplaySubject<Date>(1);
  public closeOn$: ReplaySubject<Date> = new ReplaySubject<Date>(1);

  public ngOnInit(): void {
    this.formGroup = new FormGroup({
      [DownloadFormKey.DateRange]: new FormControl(null),
    });

    this.dateRangeConfig$ = combineLatest([this.countPeriod$, this.closeOn$]).pipe(
      map(([countPeriod, closeOn]) => this.getDateRangeConfig(countPeriod, closeOn)),
      shareReplay(1),
    );

    combineLatest([this.countPeriod$, this.closeOn$])
      .pipe(take(1))
      .subscribe(([countPeriod, closeOn]) =>
        this.formGroup.get(DownloadFormKey.DateRange).patchValue({
          startDate: countPeriod,
          endDate: closeOn ?? endOfMonth(subMonths(new Date(), 1)),
        } as IDateRange));
  }

  public cancelDownload(): void {
    this.formGroup.reset();
    this.cancel.emit();
  }

  public download(): void {
    const period: IDateRange = this.formGroup.get(DownloadFormKey.DateRange).value;
    this.downloadCountsDetail.emit({
      startDate: startOfMonth(period.startDate),
      endDate: endOfMonth(period.endDate),
    });
  }

  private getDateRangeConfig(countPeriod?: Date, closeOn?: Date): IDateRangeConfiguration {
    return {
      granularity: ELuDateGranularity.month,
      periodCoverStrategy: EndDateGranularityPolicy.Beginning,
      startDateConfiguration: {
        min: countPeriod,
        max: closeOn ?? endOfMonth(subMonths(new Date(), 1)),
        class: 'palette-grey mod-outlined mod-inline mod-short',
      },
      endDateConfiguration: {
        min: countPeriod,
        max: closeOn ?? endOfMonth(subMonths(new Date(), 1)),
        class: 'palette-grey mod-outlined mod-inline mod-short',
      },
    };
  }
}
