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
import { takeUntil } from 'rxjs/operators';

import { IAdvancedCriterionForm } from '../../advanced-filter-form.interface';
import { ICriterionConfiguration } from '../../models/advanced-filter-configuration.interface';
import { ComparisonCriterion } from '../../models/comparison-criterion.interface';

enum AdvancedCriterionFormKey {
  Criterion = 'criterion',
  Content = 'content',
}

@Component({
  selector: 'cc-advanced-criterion-form',
  templateUrl: './advanced-criterion-form.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AdvancedCriterionFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: AdvancedCriterionFormComponent,
    },
  ],
})
export class AdvancedCriterionFormComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public configurations: ICriterionConfiguration[];
  @Input() public formlyFieldConfigs?: FormlyFieldConfig[];

  public formGroup: FormGroup;
  public formKey = AdvancedCriterionFormKey;

  public get criterion(): ComparisonCriterion { return this.formGroup.get(AdvancedCriterionFormKey.Criterion).value; }
  public criterionConfig$ = new ReplaySubject<ICriterionConfiguration>(1);

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.formGroup = new FormGroup({
      [AdvancedCriterionFormKey.Criterion]: new FormControl(null, Validators.required),
      [AdvancedCriterionFormKey.Content]: new FormControl(null, Validators.required),
    });
  }

  public ngOnInit(): void {
    this.formGroup.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));

    this.formGroup.get(AdvancedCriterionFormKey.Criterion).valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(criterion => this.updateConfig(criterion));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IAdvancedCriterionForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IAdvancedCriterionForm): void {
    if (!!form && this.formGroup.value !== form) {
      this.formGroup.patchValue(form);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formGroup.invalid) {
      return { invalid: true };
    }
  }

  public hasChild(configuration: ICriterionConfiguration): boolean {
    return !!configuration?.children?.length;
  }

  private updateConfig(criterion: ComparisonCriterion): void {
    const config = this.configurations.find(conf => conf.key === criterion.key);
    this.criterionConfig$.next(config);
    this.formGroup.get(AdvancedCriterionFormKey.Content).reset();
  }
}
