import { Component, forwardRef, Input, OnDestroy,OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor, FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { combineLatest, ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IComponentConfiguration } from '../../../../models/advanced-filter-configuration.interface';
import { IComparisonValue, IFormlyFieldValue } from './comparison-value.interface';

@Component({
  selector: 'cc-comparison-value-select',
  templateUrl: './comparison-value-select.component.html',
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
  @Input() public set configuration(c: IComponentConfiguration) { this.configuration$.next(c); };

  public formGroup: FormGroup = new FormGroup({});
  public model: IFormlyFieldValue = {};
  public configuration$ = new ReplaySubject<IComponentConfiguration>(1);

  private destroy$: Subject<void> = new Subject<void>();

  public ngOnInit(): void {
    combineLatest([this.formGroup.valueChanges, this.configuration$])
      .pipe(takeUntil(this.destroy$))
      .subscribe(([fieldValue, config]) => this.onChange(this.toComparisonValue(config, fieldValue)));
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

    this.clearControls();
    this.addControls(configuration?.fieldValues);
  }

  private clearControls(): void {
    const controlKeys = Object.keys(this.formGroup?.controls);
    controlKeys.forEach(key => this.formGroup.removeControl(key));
    this.formGroup.reset();
  }

  private addControls(values: IFormlyFieldValue = {}): void {
    const addedKeys = Object.keys(values);
    addedKeys.forEach(key => this.formGroup.addControl(key, new FormControl(values?.[key])));
  }

  private toComparisonValue(config: IComponentConfiguration, fieldValues: IFormlyFieldValue): IComparisonValue {
    const isEmptyOrNull = !fieldValues || !Object.keys(fieldValues).length;
    return !isEmptyOrNull ? { key: config?.key, fieldValues } : null;
  }
}
