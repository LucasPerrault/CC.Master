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
import { BehaviorSubject, combineLatest, ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { IComparisonCriterionForm } from '../../../advanced-filter-form.interface';
import { IComponentConfiguration, ICriterionConfiguration } from '../../../models/advanced-filter-configuration.interface';
import { ComparisonCriterion } from '../../../models/comparison-criterion.interface';
import { IComparisonOperator } from './comparison-operator-select/comparison-operator.interface';

enum ComparisonFilterCriterionFormKey {
  Operator = 'operator',
  Values = 'values',
}

@Component({
  selector: 'cc-comparison-filter-criterion-form',
  styles: [':host {display: block; flex: 1}'],
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
  @Input() public set criterion(criterion: ComparisonCriterion) { this.criterion$.next(criterion); }
  @Input() public set configuration(c: ICriterionConfiguration) { this.criterionConfig$.next(c); }

  public formGroup: FormGroup;
  public formKey = ComparisonFilterCriterionFormKey;

  public criterionConfig$ = new BehaviorSubject<ICriterionConfiguration>(null);
  public componentConfig$ = new BehaviorSubject<IComponentConfiguration>(null);
  private criterion$ = new ReplaySubject<ComparisonCriterion>(1);
  private allComponentConfigs$ = new ReplaySubject<IComponentConfiguration[]>(1);

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.formGroup = new FormGroup({
      [ComparisonFilterCriterionFormKey.Operator]: new FormControl(null, Validators.required),
      [ComparisonFilterCriterionFormKey.Values]: new FormControl(null, Validators.required),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));

    combineLatest([this.allComponentConfigs$, this.formGroup.get(this.formKey.Operator).valueChanges])
      .pipe(takeUntil(this.destroy$))
      .subscribe(([allComponentConfigs, operator]) => {
        this.updateComponentConfig(allComponentConfigs, operator);
        this.resetComparisonValues(allComponentConfigs, operator);
      });

    combineLatest([this.criterion$, this.criterionConfig$])
      .pipe(takeUntil(this.destroy$))
      .subscribe(([criterion, criterionConfig]) => {
        const componentConfigs = criterionConfig?.componentConfigs?.(criterion) ?? [];
        this.allComponentConfigs$.next(componentConfigs);
        this.updateValuesValidator(componentConfigs);
        this.updateDefaultOperator(criterionConfig?.operators);
      });
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IComparisonCriterionForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IComparisonCriterionForm): void {
    if (!!form && this.formGroup.value !== form) {
      this.formGroup.patchValue(form);
    }

  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return  { invalid: true };
    }
  }

  private updateDefaultOperator(operators: IComparisonOperator[]): void {
    if (!!operators?.length) {
      this.formGroup.get(ComparisonFilterCriterionFormKey.Operator).setValue(operators[0]);
    }
  }

  private updateValuesValidator(allComponentConfigs: IComponentConfiguration[]): void {
    const validators = !!allComponentConfigs?.length ? [Validators.required] : [];
    this.formGroup.get(ComparisonFilterCriterionFormKey.Values).setValidators(validators);
  }

  private resetComparisonValues(allComponentConfigs: IComponentConfiguration[], operator: IComparisonOperator) {
    const currentKey = this.formGroup.get(ComparisonFilterCriterionFormKey.Values)?.value?.key;
    const nextKey = this.getComponentConfiguration(allComponentConfigs, operator)?.key;
    if (nextKey !== currentKey) {
      this.formGroup.get(ComparisonFilterCriterionFormKey.Values).reset();
    }
  }

  private updateComponentConfig(allComponentConfigs: IComponentConfiguration[], operator: IComparisonOperator) {
    const configuration = this.getComponentConfiguration(allComponentConfigs, operator);
    this.componentConfig$.next(configuration);
  }

  private getComponentConfiguration(
    allComponentConfigs: IComponentConfiguration[],
    operator?: IComparisonOperator,
  ): IComponentConfiguration {
    const isDependedOnOperator = !!allComponentConfigs.find(c => !!c.matchingOperators?.length);

    if (!!isDependedOnOperator && !operator) {
      return;
    }

    if (!isDependedOnOperator) {
      return allComponentConfigs[0];
    }

    return allComponentConfigs.find(c => c.matchingOperators.includes(operator.id));
  }
}
