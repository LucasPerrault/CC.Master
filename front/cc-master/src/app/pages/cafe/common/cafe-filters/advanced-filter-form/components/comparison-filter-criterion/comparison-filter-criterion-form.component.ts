import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl, FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors, Validator,
} from '@angular/forms';
import { ReplaySubject, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { ComparisonOperator } from '../../enums/comparison-operator.enum';
import { ICriterionConfiguration } from '../../models/advanced-filter-configuration.interface';
import { IComparisonFilterCriterionForm } from './comparison-filter-criterion-form.interface';
import { getCriterionOperator } from './comparison-operator-select/comparison-operator.interface';

enum ComparisonFilterCriterionFormKey {
  Criterion = 'criterion',
  Operator = 'operator',
  Values = 'values',
}

@Component({
  selector: 'cc-comparison-filter-criterion-form',
  templateUrl: './comparison-filter-criterion-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ComparisonFilterCriterionFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ComparisonFilterCriterionFormComponent,
    },
  ],
})
export class ComparisonFilterCriterionFormComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public configurations: ICriterionConfiguration[];

  public configuration$: ReplaySubject<ICriterionConfiguration> = new ReplaySubject<ICriterionConfiguration>(1);

  public formGroup: FormGroup;
  public formKey = ComparisonFilterCriterionFormKey;

  private destroy$: Subject<void> = new Subject<void>();

  public ngOnInit(): void {
    this.formGroup = new FormGroup({
      [ComparisonFilterCriterionFormKey.Criterion]: new FormControl(null),
      [ComparisonFilterCriterionFormKey.Operator]: new FormControl(null),
      [ComparisonFilterCriterionFormKey.Values]: new FormControl(null),
    });

    this.formGroup.get(ComparisonFilterCriterionFormKey.Criterion).valueChanges
      .pipe(map(criterion => this.configurations?.find(c => c.key === criterion?.key)))
      .subscribe(configuration => this.configuration$.next(configuration));

    this.configuration$
      .pipe(takeUntil(this.destroy$))
      .subscribe(configuration => this.setDefaultOperator(configuration.operators));

    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(comparisonFilterCriterion => this.onChange(comparisonFilterCriterion));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (filter: IComparisonFilterCriterionForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IComparisonFilterCriterionForm): void {
    if (!!form) {
      this.formGroup.patchValue(form);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }

  private setDefaultOperator(operators: ComparisonOperator[]): void {
    const defaultSelection = getCriterionOperator(operators[0]);
    this.formGroup.get(ComparisonFilterCriterionFormKey.Operator).setValue(defaultSelection);
  }
}
