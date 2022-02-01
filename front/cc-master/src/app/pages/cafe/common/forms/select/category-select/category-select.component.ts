import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ICategory } from './category.interface';

@Component({
  selector: 'cc-category-select',
  templateUrl: './category-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CategorySelectComponent),
      multi: true,
    },
  ],
})
export class CategorySelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public placeholder: string;
  @Input() public textfieldClass: string;
  @Input() public set categories(categories: ICategory[]) {
    this.translatedCategories = categories.map(c => this.getTranslatedCategory(c));
  }

  public translatedCategories: ICategory[] = [];
  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  constructor(private translatePipe: TranslatePipe) {
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(category => this.onChange(category));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (category: ICategory) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(category: ICategory): void {
    if (!!category && category !== this.formControl.value) {
      this.formControl.setValue(this.getTranslatedCategory(category));
    }
  }

  public searchFn(category: ICategory, clue: string): boolean {
    return category.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, category: ICategory): string {
    return category.id;
  }

  private getTranslatedCategory(category: ICategory): ICategory {
    return { ...category, name: this.translatePipe.transform(category.name) };
  }
}
