import { Component, forwardRef, Input } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { TranslatePipe } from '@cc/aspects/translate';

import { attachmentEndReasons, IAttachmentEndReason } from '../../constants/attachment-end-reason.const';

@Component({
  selector: 'cc-end-reason-select',
  templateUrl: './end-reason-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EndReasonSelectComponent),
      multi: true,
    },
  ],
})
export class EndReasonSelectComponent implements ControlValueAccessor {
  @Input() required = false;
  @Input() textfieldClass?: string;

  @Input()
  public get disabled(): boolean { return this.isDisabled; }
  public set disabled(isDisabled: boolean) {
    this.setDisabledState(isDisabled);
  }

  public onChange: (reason: IAttachmentEndReason) => void;
  public onTouch: () => void;

  public model: IAttachmentEndReason;
  public endReasons: IAttachmentEndReason[];

  private isDisabled = false;

  constructor(private translatePipe: TranslatePipe) {
    this.endReasons = attachmentEndReasons.map(r => this.getTranslatedEndReason(r));
  }

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(reason: IAttachmentEndReason): void {
    if (reason !== this.model && reason !== null) {
      this.model = this.getTranslatedEndReason(reason);
    }
  }

  public setDisabledState(isDisabled: boolean) {
    this.isDisabled = isDisabled;
  }

  public update(reason: IAttachmentEndReason): void {
    this.onChange(reason);
  }

  public trackBy(index: number, reason: IAttachmentEndReason): string {
    return reason.id;
  }

  private getTranslatedEndReason(endReason: IAttachmentEndReason): IAttachmentEndReason {
    return ({ ...endReason, name: this.translatePipe.transform(endReason.name) });
  }
}
