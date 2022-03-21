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
import { BehaviorSubject, combineLatest, Observable, Subject } from 'rxjs';
import { filter, map, takeUntil } from 'rxjs/operators';

import { IComponentConfiguration, ICriterionConfiguration } from '../../models/advanced-filter-configuration.interface';
import { IComparisonFilterCriterionForm } from './comparison-filter-criterion-form.interface';
import { IComparisonOperator } from './comparison-operator-select/comparison-operator.interface';

enum ComparisonFilterCriterionFormKey {
  Criterion = 'criterion',
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
  @Input() public configurations: ICriterionConfiguration[];
  @Input() public formlyFieldConfigs?: FormlyFieldConfig[];

  public get operator$(): Observable<IComparisonOperator> {
    return this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Operator).valueChanges;
  }

  public configuration$: BehaviorSubject<ICriterionConfiguration> = new BehaviorSubject<ICriterionConfiguration>(null);
  public componentConfiguration$ = new BehaviorSubject<IComponentConfiguration>(null);

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

    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Operator).valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(operator => this.resetComparisonValues(operator));

    this.configuration$
      .pipe(takeUntil(this.destroy$))
      .subscribe(configuration => {
        this.updateValuesValidator(configuration);
        this.setDefaultOperator(configuration?.operators);
      });

    combineLatest([this.configuration$, this.operator$])
      .pipe(takeUntil(this.destroy$), map(([config, operator]) => this.getComponentConfiguration(config, operator)))
      .subscribe(configuration => this.componentConfiguration$.next(configuration));

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

  private hasChildren(config: IComparisonFilterCriterionForm): boolean {
    const configuration = this.configurations.find(c => c.key === config?.criterion?.key);
    return !!configuration?.children?.length;
  }

  private setDefaultOperator(operators: IComparisonOperator[]): void {
    if (!operators?.length) {
      return;
    }
    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Operator).setValue(operators[0]);
  }

  private updateValuesValidator(configuration: ICriterionConfiguration): void {
    const validators = !!this.getComponentConfigs(configuration).length ? [Validators.required] : [];
    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Values).setValidators(validators);
  }

  private resetComparisonValues(operator: IComparisonOperator) {
    const currentKey = this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Values)?.value?.key;
    const nextKey = this.getComponentConfiguration(this.configuration$.value, operator)?.key;

    if (nextKey === currentKey) {
      return;
    }

    this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Values).reset();
  }

  private getComponentConfigs(config: ICriterionConfiguration): IComponentConfiguration[] {
    const criterion = this.parentFormGroup.get(ComparisonFilterCriterionFormKey.Criterion).value;
    return !!config?.componentConfigs ? config?.componentConfigs(criterion) : [];
  }

  private getComponentConfiguration(configuration: ICriterionConfiguration, operator?: IComparisonOperator): IComponentConfiguration {
    const configurations = this.getComponentConfigs(configuration) ?? [];
    const isDependedOnOperator = !!configurations.find(c => !!c.matchingOperators?.length);

    if (!!isDependedOnOperator && !operator) {
      return;
    }

    if (!isDependedOnOperator) {
      return configurations[0];
    }

    return configurations.find(c => c.matchingOperators.includes(operator.id));
  }
}
