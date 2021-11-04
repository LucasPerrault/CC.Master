import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';

import { closeContractReasons } from '../../constants/close-contract-reason.enum';
import { ICloseContractReason } from '../../models/close-contract-reason.interface';

@Component({
  selector: 'cc-close-contract-reason-select',
  templateUrl: './close-contract-reason-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CloseContractReasonSelectComponent),
      multi: true,
    },
  ],
})
export class CloseContractReasonSelectComponent implements ControlValueAccessor {
  @Input() public required = false;
  @Input() public textfieldClass?: string;

  public onChange: (endContractReason: ICloseContractReason) => void;
  public onTouch: () => void;

  public endContractReason: ICloseContractReason;
  public get endReasons(): ICloseContractReason[] {
    return this.getTranslatedEndReasons(closeContractReasons);
  }

  constructor(private translatePipe: TranslatePipe) { }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(endContractReason: ICloseContractReason): void {
    if (!!endContractReason && this.endContractReason !== endContractReason) {
      this.endContractReason = endContractReason;
    }
  }

  public update(): void {
    this.onChange(this.endContractReason);
  }

  public trackBy(index: number, endReason: ICloseContractReason): number {
    return endReason.id;
  }

  private getTranslatedEndReasons(endReasons: ICloseContractReason[]): ICloseContractReason[] {
    return endReasons.map(r => ({
      ...r,
      name: this.translatePipe.transform(r.name),
    }));
  }

}
