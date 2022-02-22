import { ChangeDetectionStrategy, Component, forwardRef, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import {
  AbstractControl,
  ControlValueAccessor,
  FormControl,
  NG_VALIDATORS,
  NG_VALUE_ACCESSOR,
  ValidationErrors,
  Validator,
} from '@angular/forms';
import { IPrincipal, PRINCIPAL } from '@cc/aspects/principal';
import { IUser } from '@cc/domain/users/v4';
import { ALuApiService } from '@lucca-front/ng/api';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DemoUserApiSelectService } from './demo-user-api-select.service';

@Component({
  selector: 'cc-demo-user-api-select',
  templateUrl: './demo-user-api-select.component.html',
  styleUrls: ['./demo-user-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: ALuApiService,
      useClass: DemoUserApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DemoUserApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: DemoUserApiSelectComponent,
    },
  ],
})
export class DemoUserApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() placeholder: string;
  @Input() multiple = false;
  @Input() required = false;

  public formControl: FormControl = new FormControl();

  public api = '/api/users';

  private destroy$: Subject<void> = new Subject();

  constructor(@Inject(PRINCIPAL) public principal: IPrincipal) {}

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (user: IUser | IUser[]) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(user: IUser | IUser[]): void {
    if (!!user && user !== this.formControl.value) {
      this.formControl.patchValue(user);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, user: IUser): number {
    return user.id;
  }

  public getUserName(user: IUser): string {
    return `${ user.firstName } ${ user.lastName }`;
  }

  public isPrincipal(userId: number): boolean {
    return this.principal.id === userId;
  }
}
