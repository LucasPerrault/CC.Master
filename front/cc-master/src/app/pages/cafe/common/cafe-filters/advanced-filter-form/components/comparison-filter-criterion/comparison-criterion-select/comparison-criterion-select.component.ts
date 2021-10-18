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
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ICriterionConfiguration } from '../../../models/advanced-filter-configuration.interface';
import { IComparisonCriterion } from './comparison-criterion.interface';

@Component({
  selector: 'cc-comparison-criterion-select',
  templateUrl: './comparison-criterion-select.component.html',
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
  @Input() public required = false;
  @Input() public textfieldClass: string;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }
  @Input() public set configurations(configurations: ICriterionConfiguration[]) {
    this.comparisonCriterions = this.getComparisonCriterionSelect(configurations);
  }

  public formControl: FormControl = new FormControl();
  public comparisonCriterions: IComparisonCriterion[] = [];

  private destroy$: Subject<void> = new Subject();

  constructor(private translatePipe: TranslatePipe) {
  }

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

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.formControl.disable();
      return;
    }
    this.formControl.enable();
  }

  public searchFn(criterion: IComparisonCriterion, clue: string): boolean {
    return criterion.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, criterion: IComparisonCriterion): string {
    return criterion.key;
  }

  private getComparisonCriterionSelect(configurations: ICriterionConfiguration[]): IComparisonCriterion[] {
    return configurations?.map(c => ({
      key: c.key,
      name: this.translatePipe.transform(c.name),
    }));
  }
}
