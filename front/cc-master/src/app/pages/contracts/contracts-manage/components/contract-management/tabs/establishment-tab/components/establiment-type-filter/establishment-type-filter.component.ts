import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IListEntry, ListEntryType } from '../../models/establishment-list-entry.interface';
import { EstablishmentTypeService } from '../../services/establishment-type.service';

@Component({
  selector: 'cc-establishment-type-filter',
  templateUrl: './establishment-type-filter.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EstablishmentTypeFilterComponent),
      multi: true,
    },
  ],
})
export class EstablishmentTypeFilterComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() public allEntries: IListEntry[];

  public formControl: FormControl = new FormControl();
  public type = ListEntryType;

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (e: ListEntryType) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(e: ListEntryType): void {
    if (!!e && e !== this.formControl.value) {
      this.formControl.patchValue(e);
    }
  }

  public getByType(entries: IListEntry[], type: ListEntryType): IListEntry[] {
    return EstablishmentTypeService.getEntriesByType(entries, type);
  }
}
