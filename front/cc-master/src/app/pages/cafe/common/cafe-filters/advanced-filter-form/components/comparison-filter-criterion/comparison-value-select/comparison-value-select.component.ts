import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IComparisonValue } from './comparison-value.interface';

@Component({
  selector: 'cc-comparison-value-select',
  templateUrl: './comparison-value-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ComparisonValueSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ComparisonValueSelectComponent,
    },
  ],
})
export class ComparisonValueSelectComponent implements OnInit, OnDestroy, Validator, ControlValueAccessor {
  @Input() public configurations: FormlyFieldConfig[];

  public formGroup: FormGroup = new FormGroup({});
  public model: IComparisonValue = {};

  private destroy$: Subject<void> = new Subject<void>();

  constructor() { }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(values => this.onChange(values));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (values: IComparisonValue) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(values: IComparisonValue): void {
    if (!!values) {
      this.formGroup.patchValue(values);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (!this.configurations?.length) {
      return null;
    }

    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }
}
