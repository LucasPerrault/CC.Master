import { Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DemoFilterFormKey, IDemoFilters } from '../../models/demo-filters.interface';

@Component({
  selector: 'cc-demo-filters',
  templateUrl: './demo-filters.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DemoFiltersComponent),
      multi: true,
    },
  ],
})
export class DemoFiltersComponent implements OnInit, OnDestroy, ControlValueAccessor {

  public formGroup: FormGroup;
  public formKey = DemoFilterFormKey;

  private destroy$ = new Subject<void>();

  constructor() {
    this.formGroup = new FormGroup({
      [DemoFilterFormKey.Search]: new FormControl(),
      [DemoFilterFormKey.Author]: new FormControl(),
      [DemoFilterFormKey.Distributor]: new FormControl(),
      [DemoFilterFormKey.IsProtected]: new FormControl(),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(filters => this.onChange(filters));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (filters: IDemoFilters) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(filters: IDemoFilters): void {
    if (!!filters && filters !== this.formGroup.value) {
      this.formGroup.patchValue(filters);
    }
  }

}
