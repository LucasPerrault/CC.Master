import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy,OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IComponentConfiguration } from '../../../models/advanced-filter-configuration.interface';
import { IComparisonValue, IFormlyFieldValue } from './comparison-value.interface';

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
  @Input() public configuration: IComponentConfiguration;

  public formGroup: FormGroup = new FormGroup({});
  public model: IFormlyFieldValue = {};

  private destroy$: Subject<void> = new Subject<void>();

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(fieldValue => this.onChange(this.toComparisonValue(fieldValue)));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (configuration: IComparisonValue) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(configuration: IComparisonValue): void {
    this.reset(configuration);
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (!this.configuration?.components?.length) {
      return null;
    }

    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }

  private reset(configuration: IComparisonValue): void {
    this.model = configuration?.fieldValues ?? {};

    const controlKeys = Object.keys(this.formGroup.controls);
    controlKeys.forEach(key => this.formGroup.removeControl(key));

    this.formGroup.reset(configuration?.fieldValues ?? {});
    this.formGroup.updateValueAndValidity();
  }

  private toComparisonValue(fieldValues: IFormlyFieldValue): IComparisonValue {
    const isEmptyOrNull = !fieldValues || !Object.keys(fieldValues).length;
    return !isEmptyOrNull ? { key: this.configuration.key, fieldValues } : null;
  }
}
