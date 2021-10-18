import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl, FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { ReplaySubject, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { ICriterionConfiguration } from '../../models/advanced-filter-configuration.interface';
import { IComparisonFilterCriterionForm } from './comparison-filter-criterion-form.interface';
import { getCriterionOperator, IComparisonOperator } from './comparison-operator-select/comparison-operator.interface';

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

  public parentFormGroup: FormGroup;
  public formKey = ComparisonFilterCriterionFormKey;

  public childFormControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject<void>();

  public ngOnInit(): void {
    this.parentFormGroup = new FormGroup({
      [ComparisonFilterCriterionFormKey.Criterion]: new FormControl(null, Validators.required),
      [ComparisonFilterCriterionFormKey.Operator]: new FormControl(null, Validators.required),
      [ComparisonFilterCriterionFormKey.Values]: new FormControl(null, Validators.required),
    });

    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Criterion).valueChanges
      .pipe(map(criterion => this.configurations?.find(c => c.key === criterion?.key)))
      .subscribe(configuration => {
        this.configuration$.next(configuration);
        this.setValidators(configuration);
      });

    this.configuration$
      .pipe(takeUntil(this.destroy$), filter(configuration => !!configuration?.operators))
      .subscribe(configuration => this.setDefaultOperator(configuration.operators));

    this.parentFormGroup.valueChanges
      .pipe(takeUntil(this.destroy$), filter( c => !this.hasChildren(c)))
      .subscribe(comparisonFilterCriterion => this.onChange(comparisonFilterCriterion));

    this.childFormControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(child => this.onChange(child));
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
      this.parentFormGroup.patchValue(form);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.hasChildren(this.parentFormGroup.value) && this.childFormControl.invalid) {
      return  { invalid: true };
    }

    if (!this.hasChildren(this.parentFormGroup.value) && this.parentFormGroup.invalid) {
      return  { invalid: true };
    }
  }

  private setDefaultOperator(operators: IComparisonOperator[]): void {
    const defaultSelection = getCriterionOperator(operators[0].id);
    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Operator).setValue(defaultSelection);
  }

  private hasChildren(config: IComparisonFilterCriterionForm): boolean {
    const configuration = this.configurations.find(c => c.key === config?.criterion?.key);
    return !!configuration?.children?.length;
  }

  private setValidators(configuration: ICriterionConfiguration): void {
    const validators = !!configuration?.fields?.length ? [Validators.required] : [];

    const formControl = this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Values);
    formControl.setValidators(validators);
    formControl.updateValueAndValidity();
  }
}
