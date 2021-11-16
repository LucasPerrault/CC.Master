import { AbstractControl, FormControl, ValidationErrors, ValidatorFn } from '@angular/forms';

import { IPriceRowForm } from '../models/price-list-form.interface';
import { IPriceList } from '@cc/domain/billing/offers';
import { isEqual } from 'date-fns';

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

  public static uniqStartsOn(priceLists: IPriceList[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors => {
      const startsOn = !!control.value ? new Date(control.value) : null;
      const isUniq = priceLists.every(p => !isEqual(new Date(p.startsOn), startsOn));
      return !isUniq ? { [PriceListValidationError.UniqStartsOn]: true } : null;
    };
  }
}
