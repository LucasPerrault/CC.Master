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

import { LogicalOperator } from '../../enums/logical-operator.enum';
import { ILogicalOperator, logicalOperators } from './logical-operator.interface';

@Component({
  selector: 'cc-logical-operator-select',
  templateUrl: './logical-operator-select.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => LogicalOperatorSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: LogicalOperatorSelectComponent,
    },
  ],
})
export class LogicalOperatorSelectComponent implements OnInit, OnDestroy, ControlValueAccessor, Validator {
  @Input() public required = false;
  @Input() public textfieldClass: string;
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public formControl: FormControl = new FormControl();
  public logicOperators: ILogicalOperator[] = [];

  private destroy$: Subject<void> = new Subject();

  constructor(private translatePipe: TranslatePipe) {
    this.logicOperators = logicalOperators.map(c => this.getTranslatedLogicOperator(c));
  }

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(logicOperator => this.onChange(logicOperator));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (logicOperator: ILogicalOperator) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(logicOperator: ILogicalOperator): void {
    if (!!logicOperator && logicOperator !== this.formControl.value) {
      this.formControl.setValue(this.getTranslatedLogicOperator(logicOperator));
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

  public searchFn(logicOperator: ILogicalOperator, clue: string): boolean {
    return logicOperator.name.toLowerCase().includes(clue.toLowerCase());
  }

  public trackBy(index: number, logicOperator: ILogicalOperator): LogicalOperator {
    return logicOperator.id;
  }

  private getTranslatedLogicOperator(operator: ILogicalOperator): ILogicalOperator {
    return { ...operator, name: this.translatePipe.transform(operator.name) };
  }
}
