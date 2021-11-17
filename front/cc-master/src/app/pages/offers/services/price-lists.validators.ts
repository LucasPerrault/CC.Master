import { AbstractControl, FormControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { isEqual } from 'date-fns';

import { IPriceRowForm } from '../models/price-list-form.interface';

export enum PriceListValidationError {
  UniqStartsOn = 'uniqStartsOn',
  BoundsContinuity = 'boundsContinuity',
  Required = 'required',
}

export class PriceListsValidators {

  public static boundsContinuity(control: FormControl): ValidationErrors {
    const priceRows: IPriceRowForm[] = control.value.priceRows ?? [];
    const areAllContinuous = priceRows.every((priceList, index) => {
      const previousUpper = priceRows[index - 1]?.maxIncludedCount;
      return !!previousUpper ? previousUpper < priceRows[index]?.maxIncludedCount : true;
    });

    return areAllContinuous ? null : { [PriceListValidationError.BoundsContinuity]: true };
  }

  public static uniqStartsOn(startDates: Date[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors => {
      const startsOn = !!control.value ? new Date(control.value) : null;
      const isUniq = startDates.every(date => !isEqual(date, startsOn));
      return !isUniq ? { [PriceListValidationError.UniqStartsOn]: true } : null;
    };
  }
}
