import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { CafeCategory } from './common/enums/cafe-category.enum';
import { CafeCategoryService } from './common/services/cafe-category.service';

@Component({
  selector: 'cc-cafe',
  templateUrl: './cafe.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CafeComponent implements OnInit, OnDestroy {

  public category$ = new ReplaySubject<CafeCategory>(1);
  public category = CafeCategory;

  private destroy$ = new Subject<void>();

  constructor(private categoryService: CafeCategoryService) {}

  public ngOnInit(): void {
    this.categoryService.category$
      .pipe(takeUntil(this.destroy$))
      .subscribe(category => this.category$.next(category));

    const defaultCategory = CafeCategory.Environments;
    this.categoryService.update(defaultCategory);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
