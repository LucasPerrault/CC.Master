import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { AdvancedFilter, IAdvancedFilterConfiguration } from './advanced-filters';
import { IAdvancedFilterAndCount } from './advanced-filters/models/advanced-filter-and-count.interface';
import { ICategory } from './category-filter/category-select/category.interface';

@Component({
  selector: 'cc-cafe-filters',
  templateUrl: './cafe-filters.component.html',
  styleUrls: ['./cafe-filters.component.scss'],
})
export class CafeFiltersComponent implements OnInit, OnDestroy {
  @Input() public categories: ICategory[];
  @Input() public configurations: IAdvancedFilterConfiguration[];
  @Output() public updateFilters: EventEmitter<AdvancedFilter> = new EventEmitter<AdvancedFilter>();

  public get hasSelectedConfiguration$(): Observable<boolean> {
    return this.configuration$.pipe(map(c => !!c));
  }

  public category: FormControl = new FormControl();

  public filtersCount = 0;
  public configuration$: ReplaySubject<IAdvancedFilterConfiguration> = new ReplaySubject(1);

  public showAdvancedFilters = false;

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.category.valueChanges
      .pipe(takeUntil(this.destroy$), map(category => this.getSelectedConfiguration(category)))
      .subscribe(configuration => this.configuration$.next(configuration));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public toggleAdvancedFiltersDisplay(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  public update(form: IAdvancedFilterAndCount): void {
    this.filtersCount = form.count;
    this.updateFilters.emit(form.filter);
  }

  private getSelectedConfiguration(category: ICategory): IAdvancedFilterConfiguration {
    return this.configurations.find(configuration => configuration.categoryId === category.id);
  }
}
