import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl, FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IRange, rangeValidatorFn } from '../../validators/range-validator';
import { IRangeConfiguration } from './range-configuration.interface';

export enum RangeFormKey {
  Min = 'minRange',
  Max = 'maxRange',
}

@Component({
  selector: 'cc-range-input',
  templateUrl: './range.component.html',
  styleUrls: ['./range.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => RangeComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: RangeComponent,
    },
  ],
})
export class RangeComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() suffix: string;
  @Input() configuration: IRangeConfiguration;
  @Input() placeholder: string;
  @Input() multiple = false;
  @Input() required = false;
  @Input() formlyAttributes: FormlyFieldConfig = {};

  public formGroup: FormGroup;
  public formKey = RangeFormKey;

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.formGroup = new FormGroup({
      [RangeFormKey.Min]: new FormControl(),
      [RangeFormKey.Max]: new FormControl(),
    });

    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(range => this.onChange(range));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (range: IRange) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(range: IRange): void {
    if (!!range && range !== this.formGroup.value) {
      this.formGroup.setValue(range);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return { invalid: true };
    }
  }
}
