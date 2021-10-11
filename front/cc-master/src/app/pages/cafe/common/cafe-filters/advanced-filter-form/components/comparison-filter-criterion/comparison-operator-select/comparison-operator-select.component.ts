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
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { ComparisonOperator } from '../../../enums/comparison-operator.enum';
import { IComparisonOperator } from './comparison-operator.interface';

@Component({
  selector: 'cc-comparison-operator-select',
  templateUrl: './comparison-operator-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ComparisonOperatorSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: ComparisonOperatorSelectComponent,
    },
  ],
})
export class ComparisonOperatorSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public required = false;
  @Input() public textfieldClass: string;
  @Input() public operators: IComparisonOperator[];
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public formControl: FormControl = new FormControl();

  private destroy$: Subject<void> = new Subject();

  constructor() {
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(operator => this.onChange(operator));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (operator: IComparisonOperator) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(operator: IComparisonOperator): void {
    if (operator !== this.formControl.value) {
      this.formControl.setValue(operator.id);
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

  public searchFn(criterionOperator: IComparisonOperator, clue: string): boolean {
    return criterionOperator.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, criterionOperator: IComparisonOperator): ComparisonOperator {
    return criterionOperator.id;
  }
}
