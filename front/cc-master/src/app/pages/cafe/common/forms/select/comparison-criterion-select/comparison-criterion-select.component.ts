import { ChangeDetectionStrategy, Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ICriterionConfiguration } from '../../../components/advanced-filter-form';
import { IComparisonCriterion } from '../../../components/advanced-filter-form';

@Component({
  selector: 'cc-comparison-criterion-select',
  templateUrl: './comparison-criterion-select.component.html',
  styleUrls: ['./comparison-criterion-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ComparisonCriterionSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ComparisonCriterionSelectComponent,
    },
  ],
})
export class ComparisonCriterionSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() placeholder: string;
  @Input() multiple = false;
  @Input() required = false;
  @Input() formlyAttributes: FormlyFieldConfig = {};
  @Input() options: ICriterionConfiguration[];

  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(criterionOperator => this.onChange(criterionOperator));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (criterion: IComparisonCriterion) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(criterion: IComparisonCriterion): void {
    if (criterion !== this.formControl.value) {
      this.formControl.setValue(criterion);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return  { invalid: true };
    }
  }

  public searchFn(criterion: IComparisonCriterion, clue: string): boolean {
    return criterion.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, criterion: IComparisonCriterion): string {
    return criterion.key;
  }
}
