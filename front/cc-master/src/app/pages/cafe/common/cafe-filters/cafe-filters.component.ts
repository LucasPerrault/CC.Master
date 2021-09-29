import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { IAdvancedFilterConfiguration } from './advanced-filter-form';
import { IAdvancedFilterForm } from './advanced-filter-form/advanced-filter-form.interface';
import { ICafeFilters } from './cafe-filters.interface';
import { ICategory } from './category-filter/category-select/category.interface';

enum CafeFilterKey {
  Category = 'category',
  AdvancedFilterForm = 'advancedFilterForm',
}

@Component({
  selector: 'cc-cafe-filters',
  templateUrl: './cafe-filters.component.html',
  styleUrls: ['./cafe-filters.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CafeFiltersComponent),
      multi: true,
    },
  ],
})
export class CafeFiltersComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public categories: ICategory[];
  @Input() public configurations: IAdvancedFilterConfiguration[];

  public get filtersCount(): number {
    const filters: IAdvancedFilterForm = this.formGroup.get(CafeFilterKey.AdvancedFilterForm)?.value;
    return filters?.criterionForms?.length || 0;
  }

  public get hasSelectedConfiguration$(): Observable<boolean> {
    return this.selectedConfiguration$.pipe(map(c => !!c));
  }

  public formGroup: FormGroup;
  public formKey = CafeFilterKey;

  public selectedConfiguration$: BehaviorSubject<IAdvancedFilterConfiguration> = new BehaviorSubject(null);

  public showAdvancedFilters = false;

  private destroy$: Subject<void> = new Subject();

  constructor() {
    this.formGroup = new FormGroup({
      [CafeFilterKey.Category]: new FormControl(),
      [CafeFilterKey.AdvancedFilterForm]: new FormControl(),
    });
  }


  public ngOnInit(): void {
    this.formGroup.get(CafeFilterKey.Category).valueChanges
      .pipe(takeUntil(this.destroy$), map(category => this.getSelectedConfiguration(category)))
      .subscribe(configuration => this.selectedConfiguration$.next(configuration));

    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: ICafeFilters) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: ICafeFilters): void {
    if (!!form) {
      this.formGroup.patchValue(form);
    }
  }

  public toggleAdvancedFiltersDisplay(): void {
    this.showAdvancedFilters = !this.showAdvancedFilters;
  }

  private getSelectedConfiguration(category: ICategory): IAdvancedFilterConfiguration {
    return this.configurations.find(configuration => configuration.categoryId === category.id);
  }
}
