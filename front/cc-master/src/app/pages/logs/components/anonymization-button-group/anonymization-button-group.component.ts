import { Component, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'cc-anonymization-button-group',
  templateUrl: './anonymization-button-group.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AnonymizationButtonGroupComponent),
      multi: true,
    },
  ],
})
export class AnonymizationButtonGroupComponent implements ControlValueAccessor {
  public isAnonymized?: boolean = null;

  public onChange: (isAnonymized: boolean) => void;
  public onTouch: () => void;

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(isAnonymizedSelectionUpdated: boolean): void {
    if (isAnonymizedSelectionUpdated !== this.isAnonymized) {
      this.isAnonymized = isAnonymizedSelectionUpdated;
    }
  }

  public safeOnChange(isAnonymizedSelectionUpdated: boolean): void {
    this.onChange(isAnonymizedSelectionUpdated);
  }
}
