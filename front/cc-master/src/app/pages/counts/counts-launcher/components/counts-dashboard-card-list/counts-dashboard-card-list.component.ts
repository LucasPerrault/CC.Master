import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { BehaviorSubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

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

  public section = CountsDashboardDetailsSection;
  private selectedSection$ = new BehaviorSubject<CountsDashboardDetailsSection>(CountsDashboardDetailsSection.WithoutCount);

  private destroy$ = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.selectedSection$
      .pipe(takeUntil(this.destroy$))
      .subscribe(section => this.showSectionDetails.emit(section));
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

}
