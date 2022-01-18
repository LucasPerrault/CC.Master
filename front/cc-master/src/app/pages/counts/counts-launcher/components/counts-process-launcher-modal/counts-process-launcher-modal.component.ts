import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { EndDateGranularityPolicy, IDateRangeConfiguration } from '@cc/common/forms/select/date-range-select';
import { ELuDateGranularity } from '@lucca-front/ng/core';
import { ILuModalContent } from '@lucca-front/ng/modal';
import { addMonths, endOfMonth, startOfMonth, subMonths } from 'date-fns';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CountsProcessDataService } from '../../services/counts-process-data.service';

@Component({
  selector: 'cc-counts-process-launcher-modal',
  templateUrl: './counts-process-launcher-modal.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CountsProcessLauncherModalComponent implements ILuModalContent, OnInit, OnDestroy {
  public title = this.translatePipe.transform('counts_launcher_process_modal_title');
  public submitLabel = this.translatePipe.transform('counts_launcher_process_modal_submitLabel');
  public submitDisabled = true;

  public dateRange: FormControl = new FormControl({
    startDate: startOfMonth(subMonths(Date.now(), 1)),
    endDate: endOfMonth(subMonths(Date.now(), 1)),
  });

  public dateRangeConfiguration: IDateRangeConfiguration = {
    granularity: ELuDateGranularity.month,
    periodCoverStrategy: EndDateGranularityPolicy.End,
    startDateConfiguration: {
      min: startOfMonth(subMonths(Date.now(), 1)),
      max: startOfMonth(addMonths(Date.now(), 10)),
      class: 'palette-grey mod-outlined mod-inline mod-long',
    },
    endDateConfiguration: {
      min: startOfMonth(subMonths(Date.now(), 1)),
      max: startOfMonth(addMonths(Date.now(), 10)),
      class: 'palette-grey mod-outlined mod-inline mod-long',
    },
  };

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private processService: CountsProcessDataService, private translatePipe: TranslatePipe) { }

  public ngOnInit(): void {
    this.dateRange.statusChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.submitDisabled = this.dateRange.invalid);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public submitAction(): Observable<void> {
    const period = this.dateRange.value;
    return this.processService.launchCountProcess$(period);
  }

}
