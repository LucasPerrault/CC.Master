import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { IDateRange } from '@cc/common/date';
import { EndDateGranularityPolicy, IDateRangeConfiguration } from '@cc/common/forms/select/date-range-select';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent, LU_MODAL_DATA } from '@lucca-front/ng/modal';
import { endOfMonth, subMonths } from 'date-fns';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CountContractsDataService } from '../../services/count-contracts-data.service';
import { ICountsDetailDownloadModalData } from './counts-detail-download-modal-data.interface';

@Component({
  selector: 'cc-counts-detail-download-modal',
  templateUrl: './counts-detail-download-modal.component.html',
})
export class CountsDetailDownloadModalComponent implements OnInit, OnDestroy, ILuModalContent {
  public title = this.translatePipe.transform('front_contractPage_counts_details_download_title');
  public submitLabel = this.translatePipe.transform('front_contractPage_counts_details_download_action_label');
  public submitDisabled = true;

  public formControl: FormControl = new FormControl();
  public dateRangeConfig: IDateRangeConfiguration;

  private destroy$: Subject<void> = new Subject();

  private get min(): Date {
    return this.data.firstCountPeriod;
  }

  private get max(): Date {
    return this.data.contractCloseOn ?? endOfMonth(subMonths(new Date(), 1));
  }

  constructor(
    @Inject(LU_MODAL_DATA) public data: ICountsDetailDownloadModalData,
    private dataService: CountContractsDataService,
    private translatePipe: TranslatePipe,
  ) {
    this.setDateRangeConfig(this.min, this.max);
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.formControl.invalid);

    this.formControl.patchValue({ startDate: this.min, endDate: this.max });
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    const dateRange: IDateRange = this.formControl.value;
    return this.dataService.download$(this.data.contractId, dateRange);
  }

  private setDateRangeConfig(min: Date, max: Date): void {
    this.dateRangeConfig = {
      granularity: ELuDateGranularity.month,
      periodCoverStrategy: EndDateGranularityPolicy.Beginning,
      startDateConfiguration: {
        min,
        max,
        class: 'palette-grey mod-outlined mod-inline mod-longer',
      },
      endDateConfiguration: {
        min,
        max,
        class: 'palette-grey mod-outlined mod-inline mod-longer u-marginTopSmall',
      },
    };
  }
}
