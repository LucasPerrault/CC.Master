import { ChangeDetectionStrategy, Component, forwardRef, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { BehaviorSubject, ReplaySubject, Subject } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

enum PasswordInputType {
  Password = 'password',
  Text = 'text',
}

@Component({
  selector: 'cc-demo-password-input',
  templateUrl: './demo-password-input.component.html',
  styleUrls: ['./demo-password-input.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DemoPasswordInputComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: DemoPasswordInputComponent,
    },
  ],
})
export class DemoPasswordInputComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {

  public password: FormControl = new FormControl();
  public passwordInputType$ = new BehaviorSubject<PasswordInputType.Password | PasswordInputType.Text>(PasswordInputType.Password);
  public isActive$ = new ReplaySubject<boolean>(1);

  private destroy$: Subject<void> = new Subject();

  constructor() {}

  public ngOnInit(): void {
    this.password.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(password => this.onChange(password));

    this.passwordInputType$
      .pipe(takeUntil(this.destroy$), map(type => type === PasswordInputType.Text))
      .subscribe(this.isActive$);
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
    if (!!password && password !== this.password.value) {
      this.password.patchValue(password);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.password.invalid) {
      return { invalid: true };
    }
  }

  public toggleDisplayPassword(): void {
    const type = this.passwordInputType$.value === PasswordInputType.Password
      ? PasswordInputType.Text
      : PasswordInputType.Password;
    this.passwordInputType$.next(type);
  }
}
