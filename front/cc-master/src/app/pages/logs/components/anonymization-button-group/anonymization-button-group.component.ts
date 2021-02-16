import {Component, forwardRef} from '@angular/core';
import {ControlValueAccessor, NG_VALUE_ACCESSOR} from '@angular/forms';

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
  public isAnonymizedData: boolean;

  public onChange: (isAnonymizedData: boolean) => void;
  public onTouch: () => void;

  public registerOnChange(fn: () => void): void {
    this.onChange = fn;
  }

  public registerOnTouched(fn: () => void): void {
    this.onTouch = fn;
  }

  public writeValue(isAnonymizedSelectionUpdated: boolean): void {
    if (isAnonymizedSelectionUpdated !== this.isAnonymizedData) {
      this.isAnonymizedData = isAnonymizedSelectionUpdated;
    }
  }

  public safeOnChange(isAnonymizedSelectionUpdated: boolean): void {
    this.onChange(isAnonymizedSelectionUpdated);
  }

}
