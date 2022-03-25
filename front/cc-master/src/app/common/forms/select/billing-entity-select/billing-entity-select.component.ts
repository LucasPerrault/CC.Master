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
import { IBillingEntity } from '@cc/domain/billing/clients';
import { FormlyFieldConfig } from '@ngx-formly/core/lib/components/formly.field.config';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'cc-billing-entity-select',
  templateUrl: './billing-entity-select.component.html',
  styleUrls: ['./billing-entity-select.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => BillingEntitySelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: BillingEntitySelectComponent,
    },
  ],
})
export class BillingEntitySelectComponent implements ControlValueAccessor, Validator, OnInit, OnDestroy {
  @Input() placeholder: string;
  @Input() multiple = false;
  @Input() required = false;
  @Input() formlyAttributes: FormlyFieldConfig = {};

  public readonly api = '/api/billing-entities';

  public formControl: FormControl = new FormControl();
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

  public onChange: (d: IBillingEntity[] | IBillingEntity) => void = () => {};
  public onTouch: () => void = () => {};

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(billingEntity: IBillingEntity | IBillingEntity[]): void {
    if (!!billingEntity && billingEntity !== this.formControl.value) {
      this.formControl.patchValue(billingEntity);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, billingEntity: IBillingEntity): string {
    return billingEntity.code;
  }
}
