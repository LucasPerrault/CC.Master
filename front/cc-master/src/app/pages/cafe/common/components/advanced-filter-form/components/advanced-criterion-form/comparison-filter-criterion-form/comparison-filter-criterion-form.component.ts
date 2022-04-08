import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  forwardRef,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
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
import { BehaviorSubject, combineLatest, Observable, pipe, ReplaySubject, Subject, UnaryFunction } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { IComparisonCriterionForm } from '../../../advanced-filter-form.interface';
import { IComponentConfiguration, ICriterionConfiguration } from '../../../models/advanced-filter-configuration.interface';
import { ComparisonCriterion } from '../../../models/comparison-criterion.interface';
import { IComparisonOperator } from './comparison-operator-select/comparison-operator.interface';
import { IComparisonValue } from './comparison-value-select/comparison-value.interface';

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
export class ComparisonFilterCriterionFormComponent implements OnInit, AfterViewInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public set criterion(c: ComparisonCriterion) { this.criterion$.next(c); }
  @Input() public set criterionConfig(c: ICriterionConfiguration) { this.criterionConfig$.next(c); }

  public formGroup: FormGroup;
  public formKey = ComparisonFilterCriterionFormKey;

  public criterionConfig$ = new BehaviorSubject<ICriterionConfiguration>(null);
  public componentConfig$ = new BehaviorSubject<IComponentConfiguration>(null);
  private criterion$ = new ReplaySubject<ComparisonCriterion>(1);
  private get valueFormControl(): AbstractControl { return this.formGroup.get(this.formKey.Values); }
  private get operator$(): Observable<IComparisonOperator> { return this.formGroup.get(this.formKey.Operator).valueChanges; }

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
  }

  /**
   * We are waiting onChange() initialization before to programmatically init UI.
   */
  public ngAfterViewInit(): void {
    combineLatest([this.criterion$, this.criterionConfig$, this.operator$])
      .pipe(takeUntil(this.destroy$), this.toComponentConfig)
      .subscribe(c => this.componentConfig$.next(c));

    combineLatest([this.criterion$, this.criterionConfig$, this.operator$])
      .pipe(
        takeUntil(this.destroy$),
        this.toComponentConfig,
        filter(updatedConfig => this.valueFormControl?.value?.key !== updatedConfig?.key),
      )
      .subscribe((componentConfig) => this.resetValues(componentConfig));

    this.criterionConfig$
      .pipe(takeUntil(this.destroy$))
      .subscribe(criterionConfig => this.selectDefaultOperator(criterionConfig));
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

  private selectDefaultOperator(config: ICriterionConfiguration): void {
    const defaultSelectedOperator = config?.defaultOperator ?? config?.operators?.[0];
    this.formGroup.get(ComparisonFilterCriterionFormKey.Operator).setValue(defaultSelectedOperator);
  }

  private resetValues(updatedComponentConfig: IComponentConfiguration) {
    const componentConfigFormControl = this.formGroup.get(ComparisonFilterCriterionFormKey.Values);
    const validators = !!updatedComponentConfig ? [Validators.required] : [];
    const defaultComparisonValue = this.getDefaultComparisonValue(updatedComponentConfig);
    componentConfigFormControl.setValue(defaultComparisonValue);
    componentConfigFormControl.setValidators(validators);
    this.formGroup.updateValueAndValidity();
  }

  private getDefaultComparisonValue(config: IComponentConfiguration): IComparisonValue {
    /**
     * Default value could be a false boolean
     */
    return config?.defaultValue !== undefined
      ? { key: config.key, fieldValues: { [config.key]: config.defaultValue } }
      : null;
  }

  private get toComponentConfig():
    UnaryFunction<Observable<[ComparisonCriterion, ICriterionConfiguration, IComparisonOperator]>, Observable<IComponentConfiguration>> {
    return pipe(map(([criterion, criterionConfig, operator]) => {
      const allComponentConfigs = criterionConfig?.componentConfigs?.(criterion);
      const isDependedOnOperator = !!allComponentConfigs.find(c => !!c.matchingOperators?.length);

      if (!!isDependedOnOperator && !operator) {
        return;
      }

      if (!isDependedOnOperator) {
        return allComponentConfigs[0];
      }

      return allComponentConfigs.find(c => c.matchingOperators.includes(operator.id));
    }));
  }
}
