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
import { billingEntities, IBillingEntity } from '@cc/domain/billing/clients';
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

  public formControl: FormControl = new FormControl();
  public billingEntities: IBillingEntity[];

  private destroy$: Subject<void> = new Subject();

  constructor(private translatePipe: TranslatePipe) {
    this.billingEntities = this.getTranslatedBillingEntities(billingEntities);
  }

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

      const translatedValues = this.multiple
        ? this.getTranslatedBillingEntities(billingEntity as IBillingEntity[])
        : this.getTranslatedBillingEntity(billingEntity as IBillingEntity);

      this.formControl.patchValue(translatedValues);
    }
  }

  public validate(control: AbstractControl): ValidationErrors | null {
    if (this.formControl.invalid) {
      return { invalid: true };
    }
  }

  public trackBy(index: number, billingEntity: IBillingEntity): number {
    return billingEntity.id;
  }

  private getTranslatedBillingEntities(entities: IBillingEntity[]): IBillingEntity[] {
    return entities?.map(b => this.getTranslatedBillingEntity(b));
  }

  private getTranslatedBillingEntity(billingEntity: IBillingEntity): IBillingEntity {
    return { ...billingEntity, name: this.translatePipe.transform(billingEntity?.name) };
  }
}
