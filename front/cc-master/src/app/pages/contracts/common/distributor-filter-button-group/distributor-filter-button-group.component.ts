import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

import { DistributorFilter } from './distributor-filter.enum';

@Component({
  selector: 'cc-distributor-filter-button-group',
  templateUrl: './distributor-filter-button-group.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => DistributorFilterButtonGroupComponent),
      multi: true,
    },
  ],
})
export class DistributorFilterButtonGroupComponent implements ControlValueAccessor {
  @Input() public radioButtonsClass?: string;

  public saleTypeSelected: DistributorFilter;
  public saleType = DistributorFilter;

  public onChange: (saleType: DistributorFilter) => void;
  public onTouch: () => void;

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(saleType: DistributorFilter): void {
    if (saleType !== this.saleTypeSelected) {
      this.saleTypeSelected = saleType;
    }
  }

  public safeOnChange(saleType: DistributorFilter): void {
    this.onChange(saleType);
  }
}
