import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  forwardRef,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { AbstractControl, ControlValueAccessor, FormArray, FormControl, FormGroup, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subject } from 'rxjs';
import { filter, takeUntil } from 'rxjs/operators';

import { IAdvancedFilterForm } from './advanced-filter-form.interface';
import { IComparisonFilterCriterionForm } from './components/comparison-filter-criterion';
import { getLogicalOperator } from './components/logical-operator-select/logical-operator.interface';
import { LogicalOperator } from './enums/logical-operator.enum';
import { IAdvancedFilterConfiguration } from './models/advanced-filter-configuration.interface';

@Component({
  selector: 'cc-advanced-filter-form',
  templateUrl: './advanced-filter-form.component.html',
  styleUrls: ['./advanced-filter-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AdvancedFilterFormComponent),
      multi: true,
    },
  ],
})
export class AdvancedFilterFormComponent implements ControlValueAccessor, OnInit, OnDestroy {
  @Input() public configuration: IAdvancedFilterConfiguration;
  @Output() public cancel: EventEmitter<void> = new EventEmitter<void>();

  public get invalid(): boolean {
    return !this.formArray.dirty || this.formArray.invalid || this.isLogicalOperatorInvalid();
  }

  public logicalOperator: FormControl = new FormControl();

  public formArrayKey = 'criterions';
  public formArray = new FormArray([]);
  public formGroup: FormGroup = new FormGroup({
    [this.formArrayKey]: this.formArray,
  });

  private destroy$: Subject<void> = new Subject<void>();

  public ngOnInit(): void {
    this.formArray.valueChanges
      .pipe(takeUntil(this.destroy$), filter(forms => forms.length === 1))
      .subscribe(() => this.logicalOperator.patchValue(getLogicalOperator(LogicalOperator.And)));
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
      this.formArray.setValue(form.criterionForms);
      return;
    }

    this.reset();
  }

  public trackBy(index: number, control: AbstractControl): AbstractControl {
    return control;
  }

  public insertAt(index: number, defaultForm?: IComparisonFilterCriterionForm): void {
    this.formArray.insert(index + 1, this.getCriterionForm(defaultForm));
  }

  public removeAt(index: number) {
    this.formArray.removeAt(index);
    if (!this.formArray.length) {
      this.insertAt(0);
    }
  }

  public removeAll(): void {
    this.logicalOperator.reset();
    this.formArray.clear();
    this.insertAt(0);
  }

  public reset(): void {
    this.logicalOperator.reset();

    this.formArray.clear();
    this.init();
  }

  public cancelForm(): void {
    this.cancel.emit();
  }

  public submitAdvancedFilter(): void {
    const criterionForms = this.formGroup.get(this.formArrayKey)?.value;
    if (!criterionForms && !!criterionForms.length) {
      return;
    }

    const logicalOperator = this.logicalOperator.value;
    const advancedFilterForm: IAdvancedFilterForm = { logicalOperator, criterionForms };

    this.onChange(advancedFilterForm);
  }

  private getCriterionForm(defaultForm?: IComparisonFilterCriterionForm): FormControl {
    return new FormControl(defaultForm);
  }

  private isLogicalOperatorInvalid(): boolean {
    return this.formArray.length > 1 ? this.logicalOperator.invalid : false;
  }

  private init(): void {
    const defaultCriterion = this.configuration.criterions[0] ?? null;
    const defaultForm = { criterion: defaultCriterion, operator: null, values: null };

    this.insertAt(0, defaultForm);
  }
}
