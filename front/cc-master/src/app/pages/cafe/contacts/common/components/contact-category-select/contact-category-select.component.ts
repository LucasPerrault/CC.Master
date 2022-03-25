import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { ICategory } from '../../../../common/forms/select/category-select/category.interface';
import { categories, getCategory } from '../../enums/cafe-contacts-category.enum';
import { CafeContactCategoryService } from '../../services/cafe-contact-category.service';

@Component({
  selector: 'cc-contact-category-select',
  templateUrl: './contact-category-select.component.html',
})
export class ContactCategorySelectComponent implements OnInit, OnDestroy {

  public categories: ICategory[] = [];
  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  constructor(private translatePipe: TranslatePipe, private categoryService: CafeContactCategoryService) {
    this.categories = categories.map(c => this.getTranslatedCategory(c));
  }

  public ngOnInit(): void {
    this.categoryService.category$
      .pipe(takeUntil(this.destroy$), filter(c => c !== this.formControl.value?.id))
      .subscribe(category => this.formControl.setValue(getCategory(category)));

    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(category => this.categoryService.update(category?.id));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
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
