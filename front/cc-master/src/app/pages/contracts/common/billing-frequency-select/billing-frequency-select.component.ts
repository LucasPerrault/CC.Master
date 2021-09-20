import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ContractBillingMonth, contractBillingMonths, IContractBillingMonth } from '@cc/domain/billing/contracts';

enum BillingFrequency {
  Quarterly,
  Yearly,
}

@Component({
  selector: 'cc-billing-frequency-select',
  templateUrl: './billing-frequency-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => BillingFrequencySelectComponent),
      multi: true,
    },
  ],
})
export class BillingFrequencySelectComponent implements ControlValueAccessor {
  @Input() public textfieldClass?: string;
  @Input() public required = false;
  @Input() public frequencyDisabled = false;

  @Input()
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public get isFrequencyDisabled(): boolean {
    return this.areAllDisabled ? this.areAllDisabled : this.frequencyDisabled;
  }

  public get isMonthDisabled(): boolean {
    return this.areAllDisabled;
  }

  public onChange: (billingMonth: number) => void;
  public onTouch: () => void;

  public frequency: BillingFrequency;
  public billingFrequency = BillingFrequency;
  public monthSelected: IContractBillingMonth;

  private areAllDisabled = false;

  public get isYearlyFrequencySelected(): boolean {
    return !!this.frequency && this.frequency === BillingFrequency.Yearly;
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(month: ContractBillingMonth): void {
    if (!month) {
      this.frequency = null;
      this.monthSelected = null;
      return;
    }

    if (month === ContractBillingMonth.Quarterly) {
      this.frequency = BillingFrequency.Quarterly;
      return;
    }

    this.frequency = BillingFrequency.Yearly;
    this.monthSelected = this.getMonthSelected(month);
  }

  public setDisabledState(isDisabled: boolean) {
    this.areAllDisabled = isDisabled;
  }

  public update(): void {
    if (this.frequency === BillingFrequency.Quarterly) {
      this.resetBillingMonthSelected();
      this.onChange(ContractBillingMonth.Quarterly);
      return;
    }

    this.onChange(this.monthSelected?.id);
  }

  private resetBillingMonthSelected(): void {
    this.monthSelected = null;
  }

  private getMonthSelected(month: ContractBillingMonth): IContractBillingMonth {
    return contractBillingMonths.find(m => m.id === month);
  }

}
