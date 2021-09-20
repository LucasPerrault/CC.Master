import { AbstractControl, ValidatorFn } from '@angular/forms';

import { IRange } from './range.interface';
import { IRangeValidationError } from './range-validation-error.interface';

export const rangeValidatorFn = (range: IRange): ValidatorFn =>
  (control: AbstractControl): IRangeValidationError | null => {
    if (!control || !control.value) {
      return null;
    }

    const hasRangeError = control.value < range.min || control.value > range.max;
    return hasRangeError && { range: hasRangeError };
  };
