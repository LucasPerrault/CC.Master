import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormArray,
  FormControl,
  FormGroup,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { IAdvancedCriterionForm, IAdvancedFilterForm } from './advanced-filter-form.interface';
import { AdvancedFilterFormService } from './advanced-filter-form.service';
import { getLogicalOperator } from './components/logical-operator-select/logical-operator.interface';
import { LogicalOperator } from './enums/logical-operator.enum';
import { IAdvancedFilterConfiguration } from './models/advanced-filter-configuration.interface';

@Component({
  selector: 'cc-advanced-filter-form',
  templateUrl: './advanced-filter-form.component.html',
  styleUrls: ['./advanced-filter-form.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AdvancedFilterFormComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: AdvancedFilterFormComponent,
    },
  ],
})
export class AdvancedFilterFormComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public configuration: IAdvancedFilterConfiguration;

  public logicalOperator: FormControl = new FormControl();

  public formArrayKey = 'criterions';
  public formArray = new FormArray([]);
  public formGroup: FormGroup = new FormGroup({
    [this.formArrayKey]: this.formArray,
  });

  private destroy$: Subject<void> = new Subject<void>();

  constructor(private formService: AdvancedFilterFormService) {}

  public ngOnInit(): void {
    this.formArray.valueChanges
      .pipe(takeUntil(this.destroy$), filter(forms => forms.length === 1))
      .subscribe(() => this.logicalOperator.patchValue(getLogicalOperator(LogicalOperator.And)));

    this.formGroup.get(this.formArrayKey).valueChanges
      .pipe(takeUntil(this.destroy$), filter(c => !!c && !!c?.length))
      .subscribe(criterionForms => this.onChange({
        logicalOperator: this.logicalOperator.value,
        criterionForms,
      }));

    this.formService.reset$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.reset());
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (form: IAdvancedFilterForm) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(form: IAdvancedFilterForm): void {
    const logicalOperator = form?.logicalOperator;
    if (!!logicalOperator && this.logicalOperator.value !== logicalOperator) {
      this.logicalOperator.setValue(logicalOperator);
    }

    const criterionForms = form?.criterionForms;
    if (!!criterionForms?.length) {
      this.formArray.clear();
      this.formArray.reset();
      this.addRange(criterionForms);
      return;
    }

    this.reset();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, control: AbstractControl): AbstractControl {
    return control;
  }

  public insertAt(index: number, defaultForm?: IAdvancedCriterionForm): void {
    this.formArray.insert(index + 1, this.getCriterionForm(defaultForm));
  }

  public removeAt(index: number) {
    this.formArray.removeAt(index);
    if (!this.formArray.length) {
      this.resetFormArray();
    }
  }

  private reset(): void {
    this.logicalOperator.reset();
    this.resetFormArray();
  }

  private resetFormArray(): void {
    this.formArray.clear();
    this.formArray.reset();
    this.insertAt(0);
  }

  private addRange(criterionForms: IAdvancedCriterionForm[]): void {
    criterionForms.forEach((f, index) => this.insertAt(index, f));
  }

  private getCriterionForm(defaultForm?: IAdvancedCriterionForm): FormControl {
    return new FormControl(defaultForm);
  }

  private get invalid(): boolean {
    const isLogicalOperatorInvalid = this.formArray.length > 1 ? this.logicalOperator.invalid : false;
    return !this.formArray.dirty
      || this.formArray.invalid
      || this.formArray.controls.some(c => c.pristine)
      || isLogicalOperatorInvalid;
  }
}
