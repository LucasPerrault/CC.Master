import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { categories, getCategory } from '../../enums/cafe-category.enum';
import { ICategory } from '../../forms/select/category-select/category.interface';
import { CafeCategoriesService } from '../../services/cafe-categories.service';
import { IAdvancedFilterConfiguration, IAdvancedFilterForm } from '../advanced-filter-form';
import { AdvancedFilterFormService } from '../advanced-filter-form/advanced-filter-form.service';

@Component({
  selector: 'cc-cafe-page-filter-template',
  templateUrl: './cafe-page-filter-template.component.html',
  styleUrls: ['./cafe-page-filter-template.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CafePageFilterTemplateComponent),
      multi: true,
    },
  ],
})
export class CafePageFilterTemplateComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public advancedFilterConfig: IAdvancedFilterConfiguration;

  public get filterCount(): number {
    return this.advancedFilter.value?.criterionForms?.filter(c => !!c)?.length ?? 0;
  }

  public get filterValid(): boolean {
    return this.advancedFilter.dirty && this.advancedFilter.valid;
  }

  public categories: ICategory[] = categories;
  public category: FormControl = new FormControl();

  public advancedFilter: FormControl = new FormControl();

  public showAdvancedFilter = true;

  private destroy$ = new Subject<void>();

  constructor(
    private categoryService: CafeCategoriesService,
    private advancedFilterService: AdvancedFilterFormService,
  ) { }

  public ngOnInit(): void {
    this.categoryService.category$
      .pipe(
        takeUntil(this.destroy$),
        filter(c => c !== this.category.value?.id),
        map(categoryId => getCategory(categoryId)))
      .subscribe(category => this.category.setValue(category));

    this.category.valueChanges
      .pipe(takeUntil(this.destroy$), filter(c => !!c))
      .subscribe(category => this.categoryService.update(category.id));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IAdvancedFilterForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IAdvancedFilterForm): void {
    if (!!form) {
      this.advancedFilter.patchValue(form);
    }
  }

  public toggleFilterDisplay(): void {
    this.showAdvancedFilter = !this.showAdvancedFilter;
  }

  public cancel(): void {
    this.showAdvancedFilter = false;
  }

  public submit(): void {
    this.onChange(this.advancedFilter.value);
  }

  public reset(): void {
    this.advancedFilterService.reset();
  }

  public openDocumentation(): void {
    const externalDocumentationUrl = 'https://support.lucca.fr/hc/fr/articles/4409676130578-Module-de-rapport-CC';
    window.open(externalDocumentationUrl);
  }

}
