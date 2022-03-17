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
import { ALuApiService } from '@lucca-front/ng/api';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { InstancesApiRoute } from '../../../constants/instance-api-route.const';
import { IDemoInstanceUser } from './demo-instance-user.interface';
import { DemoInstanceUserApiSelectService } from './demo-instance-user-api-select.service';

@Component({
  selector: 'cc-demo-instance-user-api-select',
  templateUrl: './demo-instance-user-api-select.component.html',
  styleUrls: ['./demo-instance-user-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: ALuApiService,
      useClass: DemoInstanceUserApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DemoInstanceUserApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: DemoInstanceUserApiSelectComponent,
    },
  ],
})
export class DemoInstanceUserApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() instanceId: number;
  @Input() placeholder: string;
  @Input() required = false;

  public formControl: FormControl = new FormControl();

  public get api(): string {
    return !!this.instanceId ? InstancesApiRoute.users(this.instanceId) : null;
  }

  private destroy$: Subject<void> = new Subject();

  public ngOnInit(): void {
    this.formControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(form => this.onChange(form));
  }

  public ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  public onChange: (user: IDemoInstanceUser) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(user: IDemoInstanceUser): void {
    if (!!user && user !== this.formControl.value) {
      this.formControl.patchValue(user);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, user: IDemoInstanceUser): number {
    return user.id;
  }

  public getUserName(user: IDemoInstanceUser): string {
    return `${ user.firstName } ${ user.lastName }`;
  }
}
