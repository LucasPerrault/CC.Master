import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

import { DistributorFilter } from '../../../common/distributor-filter-button-group';
import { IContractsDraftFilter } from '../../models';

@Component({
  selector: 'cc-contracts-draft-filter',
  templateUrl: './contracts-draft-filter.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => ContractsDraftFilterComponent),
      multi: true,
    },
  ],
})
export class ContractsDraftFilterComponent implements ControlValueAccessor {
  public onChange: (draftsFilter: IContractsDraftFilter) => void;
  public onTouch: () => void;

  public draftsFilter: IContractsDraftFilter = {
    saleType: DistributorFilter.All,
    draftName: null,
  };

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(draftsFilter: IContractsDraftFilter): void {
    if (!draftsFilter) {
      return;
    }

    this.onChange(draftsFilter);
    this.draftsFilter = draftsFilter;
  }

  public update(): void {
    this.onChange(this.draftsFilter);
  }
}
