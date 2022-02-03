import { Component, EventEmitter, forwardRef, Input, OnDestroy, OnInit, Output } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
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
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: CafePageFilterTemplateComponent,
    },
  ],
})
export class CafePageFilterTemplateComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public advancedFilterConfig: IAdvancedFilterConfiguration;
  @Input() public submitDisabled: boolean;
  // eslint-disable-next-line @angular-eslint/no-output-native
  @Output() public submit = new EventEmitter<void>();

  public get filterCount(): number {
    return this.advancedFilter.value?.criterionForms?.filter(c => !!c)?.length ?? 0;
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

    this.advancedFilter.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(f => this.onChange(f));
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

  public validate(control: AbstractControl): ValidationErrors | null {
    if (!this.advancedFilter.dirty || !this.advancedFilter.valid) {
      return { invalid: true };
    }
  }

  public toggleFilterDisplay(): void {
    this.showAdvancedFilter = !this.showAdvancedFilter;
  }

  public cancel(): void {
    this.showAdvancedFilter = false;
  }

  public submitForm(): void {
    this.submit.emit();
  }

  public reset(): void {
    this.advancedFilterService.reset();
  }

  public openDocumentation(): void {
    const externalDocumentationUrl = 'https://support.lucca.fr/hc/fr/articles/4409676130578-Module-de-rapport-CC';
    window.open(externalDocumentationUrl);
  }

}
