import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { TranslatePipe } from '@cc/aspects/translate';
import { BehaviorSubject, Observable, pipe, Subject, UnaryFunction } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { CountsDashboardDetailsSection } from '../../enums/counts-dashboard-details-section.enum';
import { CountsDashboard } from '../../models/counts-dashboard.interface';

@Component({
  selector: 'cc-counts-dashboard-card-list',
  templateUrl: './counts-dashboard-card-list.component.html',
  styleUrls: ['./counts-dashboard-card-list.component.scss'],
})
export class CountsDashboardCardListComponent implements OnInit, OnDestroy {
  @Input() public dashboard: CountsDashboard;
  @Output() public showSectionDetails = new EventEmitter<CountsDashboardDetailsSection>();

  public contractsTitle$: Observable<string>;
  public realCountsTitle$: Observable<string>;
  public missingCountsTitle$: Observable<string>;
  public draftCountsTitle$: Observable<string>;
  public countsWithoutAccountingEntryTitle$: Observable<string>;

  public section = CountsDashboardDetailsSection;
  private selectedSection$ = new BehaviorSubject<CountsDashboardDetailsSection>(CountsDashboardDetailsSection.WithoutCount);

  private destroy$ = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) { }

  public ngOnInit(): void {
    this.selectedSection$
      .pipe(takeUntil(this.destroy$))
      .subscribe(section => this.showSectionDetails.emit(section));

    this.contractsTitle$ = this.dashboard.numberOfContracts$
      .pipe(this.toTitle('counts_launcher_number_of_contracts'));
    this.realCountsTitle$ = this.dashboard.numberOfContracts$
      .pipe(this.toTitle('counts_launcher_number_of_real_counts'));
    this.missingCountsTitle$ = this.dashboard.numberOfContracts$
      .pipe(this.toTitle('counts_launcher_missing_counts'));
    this.draftCountsTitle$ = this.dashboard.numberOfContracts$
      .pipe(this.toTitle('counts_launcher_draft_counts'));
    this.countsWithoutAccountingEntryTitle$ = this.dashboard.numberOfContracts$
      .pipe(this.toTitle('counts_launcher_without_accounting_entry'));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public showDetails(section: CountsDashboardDetailsSection): void {
    this.selectedSection$.next(section);
  }

  public isSelected(section: CountsDashboardDetailsSection): boolean {
    return this.selectedSection$.value === section;
  }

  private toTitle(translationKey: string): UnaryFunction<Observable<number>, Observable<string>> {
    return pipe(map(totalCount => this.translatePipe.transform(translationKey, { count: totalCount ?? 0 })));
  }

}
