import { Component, forwardRef, Input, OnDestroy, OnInit } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-password-input',
  templateUrl: './password-input.component.html',
  styleUrls: ['./password-input.component.scss'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PasswordInputComponent),
      multi: true,
    },
  ],
})
export class PasswordInputComponent implements OnInit, OnDestroy, ControlValueAccessor {
  @Input() readonly = false;
  @Input() required = false;
  @Input() placeholder = '';
  @Input() public set disabled(isDisabled: boolean) { this.setDisabledState(isDisabled); }

  public password = new FormControl();
  public showPassword = false;
  public get type(): string { return this.showPassword ? 'text' : 'password'; }

  private destroy$ = new Subject<void>();

  constructor(private translatePipe: TranslatePipe) {}

  public ngOnInit(): void {
    this.password.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(password => this.onChange(password));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (password: string) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(password: string): void {
    this.password.patchValue(password);
  }

  public setDisabledState(isDisabled: boolean) {
    if (isDisabled) {
      this.password.disable();
      return;
    }
    this.password.enable();
  }

  public toggle(): void {
    this.showPassword = !this.showPassword;
  }
}
