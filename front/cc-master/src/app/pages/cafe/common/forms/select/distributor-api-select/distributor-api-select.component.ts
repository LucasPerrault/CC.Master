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
import { IDistributor } from '@cc/domain/billing/distributors';
import { ALuApiService } from '@lucca-front/ng/api';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { DistributorApiSelectService } from './distributor-api-select.service';


@Component({
  selector: 'cc-distributor-api-select',
  templateUrl: './distributor-api-select.component.html',
  styleUrls: ['./distributor-api-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: ALuApiService,
      useClass: DistributorApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DistributorApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: DistributorApiSelectComponent,
    },
  ],
})
export class DistributorApiSelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() placeholder: string;
  @Input() multiple = false;
  @Input() required = false;
  @Input() formlyAttributes: FormlyFieldConfig = {};

  public formControl: FormControl = new FormControl();

  public apiUrl = '/api/cafe/distributors';

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

  public onChange: (d: IDistributor[] | IDistributor) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(d: IDistributor | IDistributor[]): void {
    if (!!d && d !== this.formControl.value) {
      this.formControl.patchValue(d);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, distributor: IDistributor): number {
    return distributor.id;
  }
}
