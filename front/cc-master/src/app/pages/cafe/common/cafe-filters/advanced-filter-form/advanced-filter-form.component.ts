import { ChangeDetectionStrategy, Component, EventEmitter, forwardRef, Input, Output } from '@angular/core';
import {
  AbstractControl, ControlValueAccessor,
  FormArray,
  FormControl,
  FormGroup,
  NG_VALUE_ACCESSOR,
} from '@angular/forms';

import { IAdvancedFilterForm } from './advanced-filter-form.interface';
import { IAdvancedFilterConfiguration } from './models/advanced-filter-configuration.interface';

enum CriterionFormsKey {
  ComparisonFilterCriterionForm = 'criterionForm',
}

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
export class AdvancedFilterFormComponent implements ControlValueAccessor {
  @Input() public configuration: IAdvancedFilterConfiguration;
  @Output() public cancel: EventEmitter<void> = new EventEmitter<void>();

  public get invalid(): boolean {
    const hasLogicalOperatorInvalid = this.formArray.length > 1 && this.logicalOperator.invalid;
    return this.formArray.invalid || hasLogicalOperatorInvalid;
  }

  public logicalOperator: FormControl = new FormControl();

  public formArrayKey = 'criterions';
  public formArray: FormArray = new FormArray([]);
  public formGroup: FormGroup = new FormGroup({
    [this.formArrayKey]: this.formArray,
  });

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

    this.add();
  }

  public trackBy(index: number, control: AbstractControl): number {
    return index;
  }

  public add(): void {
    this.formArray.push(new FormControl({
      [CriterionFormsKey.ComparisonFilterCriterionForm]: null,
    }));
  }

  public remove(index: number) {
    this.formArray.removeAt(index);
  }

  public clear(): void {
    this.logicalOperator.reset();
    this.formArray.clear();
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
}
