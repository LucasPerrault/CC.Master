import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { contractBillingMonths, IContractBillingMonth } from '@cc/domain/billing/contracts';

@Component({
  selector: 'cc-billing-month-select',
  templateUrl: './billing-month-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => BillingMonthSelectComponent),
      multi: true,
    },
  ],
})
export class BillingMonthSelectComponent implements ControlValueAccessor {
  @Input() public textfieldClass?: string;
  @Input() public disabled = false;
  @Input() public required = false;

  public onChange: (month: IContractBillingMonth) => void;
  public onTouch: () => void;

  public monthSelected: IContractBillingMonth;

  public get months(): IContractBillingMonth[] {
    return contractBillingMonths;
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(month: IContractBillingMonth): void {
    if (month !== this.monthSelected) {
      this.monthSelected = month;
    }
  }

  public update(month: IContractBillingMonth): void {
    this.monthSelected = month;
    this.onChange(month);
  }

  public trackBy(index: number, month: IContractBillingMonth): number {
    return month.id;
  }
}
