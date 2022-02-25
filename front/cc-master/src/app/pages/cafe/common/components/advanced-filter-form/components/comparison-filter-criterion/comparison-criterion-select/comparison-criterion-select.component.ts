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

import { AdvancedFilterKey } from '../../../../../services/criterion-formly-configuration.service';
import { ComparisonCriterion } from '../../../models/comparison-criterion.interface';

@Component({
  selector: 'cc-comparison-criterion-select',
  templateUrl: './comparison-criterion-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ComparisonCriterionSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ComparisonCriterionSelectComponent,
    },
  ],
})
export class ComparisonCriterionSelectComponent implements OnInit, OnDestroy, Validator, ControlValueAccessor {
  @Input() public fields: FormlyFieldConfig[];

  public formGroup: FormGroup = new FormGroup({});
  public model: { [AdvancedFilterKey.Criterion]: ComparisonCriterion } = { [AdvancedFilterKey.Criterion]: null };

  private destroy$: Subject<void> = new Subject<void>();

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form?.criterion));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (configuration: ComparisonCriterion) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(criterion: ComparisonCriterion): void {
    this.model = { [AdvancedFilterKey.Criterion]: criterion };
    this.formGroup.reset(criterion);
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (!this.fields?.length) {
      return null;
    }

    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }
}
