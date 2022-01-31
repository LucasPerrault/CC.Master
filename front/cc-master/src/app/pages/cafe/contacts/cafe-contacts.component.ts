import { ChangeDetectionStrategy, Component, OnDestroy, OnInit } from '@angular/core';
import { ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ContactCategory } from './common/enums/cafe-contacts-category.enum';
import { CafeContactCategoryService } from './common/services/cafe-contact-category.service';

@Component({
  selector: 'cc-cafe-contacts',
  templateUrl: './cafe-contacts.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['./cafe-contacts.component.scss'],
})
export class CafeContactsComponent implements OnInit, OnDestroy {
  public category$ = new ReplaySubject<ContactCategory>(1);
  public contactCategory = ContactCategory;

  private destroy$ = new Subject<void>();

  constructor(private categoryService: CafeContactCategoryService) {}

  public ngOnInit(): void {
    this.categoryService.category$
      .pipe(takeUntil(this.destroy$))
      .subscribe(category => this.category$.next(category));

    const defaultCategory = ContactCategory.Application;
    this.categoryService.update(defaultCategory);
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
