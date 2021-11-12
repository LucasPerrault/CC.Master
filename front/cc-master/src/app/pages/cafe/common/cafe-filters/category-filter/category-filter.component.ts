import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { ICategory } from './category-select/category.interface';

@Component({
  selector: 'cc-category-filter',
  templateUrl: './category-filter.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CategoryFilterComponent),
      multi: true,
    },
  ],
})
export class CategoryFilterComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public categories: ICategory[];

  public get parent(): ICategory {
    return this.parentCategory.value;
  }

  public parentCategory: FormControl = new FormControl();
  public childCategory: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  constructor() { }

  public ngOnInit(): void {
    this.parentCategory.valueChanges
      .pipe(takeUntil(this.destroy$), filter(parent => !this.hasChildren(parent)))
      .subscribe(parent => this.onChange(parent));

    this.parentCategory.valueChanges
      .pipe(takeUntil(this.destroy$), filter(parent => this.hasChildren(parent)))
      .subscribe(parent => {
        const defaultSelectedChild = parent.children[0];
        this.childCategory.setValue(defaultSelectedChild);
      });

    this.childCategory.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(child => this.onChange(child));
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
    if (!category) {
      return;
    }

    if (this.isParent(category)) {
      this.parentCategory.setValue(category);
      return;
    }

    const parent = this.getParent(category);
    this.parentCategory.setValue(parent);
    this.childCategory.setValue(category);
  }

  private getParent(category: ICategory): ICategory {
    return this.categories.find(parent => this.isChild(parent, category));
  }

  private isParent(category: ICategory): boolean {
    return !!this.categories.find(c => c.id === category.id);
  }

  private isChild(parent: ICategory, child: ICategory): boolean {
    if (!this.hasChildren(parent)) {
      return false;
    }

    return !!parent.children.find(c => c.id === child.id);
  }

  private hasChildren(parent: ICategory): boolean {
    return !!parent?.children?.length;
  }

}
