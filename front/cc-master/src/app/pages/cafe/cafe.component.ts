import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CafeConfiguration } from './cafe-configuration';
import { ICafeConfiguration } from './cafe-configuration.interface';
import { CafeExportService } from './cafe-export.service';
import { IAdvancedFilterForm } from './common/cafe-filters/advanced-filter-form';
import { ICategory } from './common/cafe-filters/category-filter/category-select/category.interface';
import { ContactCategory } from './contacts/common/enums/cafe-contacts-category.enum';
import { EnvironmentsCategory } from './environments/enums/environments-category.enum';

@Component({
  selector: 'cc-cafe',
  templateUrl: './cafe.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CafeComponent implements OnInit, OnDestroy {

  public cafeFilters: FormControl = new FormControl();
  public configuration: ICafeConfiguration;

  public isExporting = false;

  public get category(): ContactCategory | EnvironmentsCategory {
    return this.cafeFilters.value?.category?.id;
  }

  public get advancedFilterForm(): IAdvancedFilterForm {
    return this.cafeFilters.value?.advancedFilterForm;
  }

  public get isEnvironmentCategory(): boolean {
    return this.category === EnvironmentsCategory.Environments;
  }


  public get isContactCategory(): boolean {
    return !!this.contactCategories.find(c => c === this.category);
    }

  private readonly contactCategories = [
    ContactCategory.Client,
    ContactCategory.Specialized,
    ContactCategory.Application,
  ];

  private destroy$: Subject<void> = new Subject();

  constructor(cafeConfiguration: CafeConfiguration, private cafeExportService: CafeExportService) {
    this.configuration = cafeConfiguration;

    this.cafeFilters.patchValue({ category: this.getDefaultCategory(EnvironmentsCategory.Environments) });
  }

  public ngOnInit(): void {
    this.cafeExportService.isExporting$
        .pipe(takeUntil(this.destroy$))
        .subscribe(isExporting => this.isExporting = isExporting);
  }

  ngOnDestroy(): void {
  this.destroy$.next();
  this.destroy$.complete();
  }

  public canExport(): boolean {
    return !!this.advancedFilterForm && !this.isExporting;
  }

  public export(): void {
    this.cafeExportService.requestExport();
  }

  private getDefaultCategory(category: EnvironmentsCategory | ContactCategory): ICategory {
      return this.configuration.categories
            .map(c => [c, ...(c.children || [])])
            .reduce((flattened: ICategory[], array: ICategory[]) => [...flattened, ...array])
            .find(c => c.id === category);
  }
}
