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
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { ReplaySubject, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { ICriterionConfiguration } from '../../models/advanced-filter-configuration.interface';
import { IComparisonFilterCriterionForm } from './comparison-filter-criterion-form.interface';
import { IComparisonOperator } from './comparison-operator-select/comparison-operator.interface';

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

  constructor() {
    this.parentFormGroup = new FormGroup({
      [ComparisonFilterCriterionFormKey.Criterion]: new FormControl(null, Validators.required),
      [ComparisonFilterCriterionFormKey.Operator]: new FormControl(null, Validators.required),
      [ComparisonFilterCriterionFormKey.Values]: new FormControl(null, Validators.required),
    });
  }

  public ngOnInit(): void {
    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Criterion).valueChanges
      .pipe(takeUntil(this.destroy$), map(criterion => this.configurations?.find(c => c.key === criterion?.key)))
      .subscribe(configuration => this.configuration$.next(configuration));

    this.configuration$
      .pipe(takeUntil(this.destroy$))
      .subscribe(configuration => {
        this.setDefaultOperator(configuration?.operators);
        this.setDefaultValues(configuration?.fields);
      });

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
    if (!form) {
      return;
    }

    this.parentFormGroup.patchValue(form);

    if (this.hasChildren(form)) {
      this.initCriterionChild(form.criterion);
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

  private initCriterionChild(criterion: ICriterionConfiguration): void {
    const defaultChildCriterion = criterion.children[0];
    this.childFormControl.patchValue({ [ComparisonFilterCriterionFormKey.Criterion]: defaultChildCriterion });
  }

  private setDefaultOperator(operators: IComparisonOperator[]): void {
    if (!operators?.length) {
      return;
    }
    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Operator).setValue(operators[0]);
  }

  private setDefaultValues(fields: FormlyFieldConfig[]): void {
    const validators = !!fields?.length ? [Validators.required] : [];

    const formControl = this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Values);
    formControl.setValue(null);
    formControl.setValidators(validators);
    formControl.updateValueAndValidity();
  }

  private hasChildren(config: IComparisonFilterCriterionForm): boolean {
    const configuration = this.configurations.find(c => c.key === config?.criterion?.key);
    return !!configuration?.children?.length;
  }
}
