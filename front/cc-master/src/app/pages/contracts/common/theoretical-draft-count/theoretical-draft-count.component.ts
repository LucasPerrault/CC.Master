import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
  Validators,
} from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-theoretical-draft-count',
  styleUrls: ['./theoretical-draft-count.component.scss'],
  templateUrl: './theoretical-draft-count.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TheoreticalDraftCountComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: TheoreticalDraftCountComponent,
    },
  ],
})
export class TheoreticalDraftCountComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() public textfieldClass?: string;
  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (theoreticalDraftCount: number) => void;
  public onTouch: () => void;

  public theoreticalDraftCount: FormControl;

  public get hasRequiredError(): boolean {
    return this.theoreticalDraftCount.touched && this.theoreticalDraftCount.hasError('required');
  }

  public get hasMinError(): boolean {
    return this.theoreticalDraftCount.hasError('min');
  }

  private destroy$: Subject<void> = new Subject<void>();

  constructor() {
    this.theoreticalDraftCount = new FormControl(null, [Validators.min(0), Validators.required]);
  }

  public ngOnInit(): void {
    this.theoreticalDraftCount.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(c => this.onChange(c));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(theoreticalDraftCount: number): void {
    if (theoreticalDraftCount !== this.theoreticalDraftCount.value) {
      this.theoreticalDraftCount.setValue(theoreticalDraftCount, { emitEvent: false });
    }
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.theoreticalDraftCount.disable();
      return;
    }
    this.theoreticalDraftCount.enable();
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.hasMinError || this.hasRequiredError) {
      return { invalid: true };
    }
  }
}
