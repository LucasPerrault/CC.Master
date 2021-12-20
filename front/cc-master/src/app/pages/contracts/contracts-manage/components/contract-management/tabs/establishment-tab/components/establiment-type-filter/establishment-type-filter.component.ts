import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { EstablishmentType } from '../../constants/establishment-type.enum';
import { IEstablishmentsWithAttachmentsByType } from '../../models/establishments-by-type.interface';

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
  @Input() public establishmentsByType: IEstablishmentsWithAttachmentsByType;

  public formControl: FormControl = new FormControl();
  public type = EstablishmentType;

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

  public onChange: (e: EstablishmentType) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(e: EstablishmentType): void {
    if (!!e && e !== this.formControl.value) {
      this.formControl.patchValue(e);
    }
  }
}
