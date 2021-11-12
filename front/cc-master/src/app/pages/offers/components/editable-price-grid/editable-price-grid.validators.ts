import { FormControl, ValidationErrors } from '@angular/forms';

import { IPriceRowForm } from '../../models/price-list-form.interface';

export enum PriceGridValidationError {
  BoundsContinuity = 'boundsContinuity',
  Required = 'required',
}

export class EditablePriceGridValidators {

  public static boundsContinuity(control: FormControl): ValidationErrors {
    const priceRows: IPriceRowForm[] = control.value.priceRows ?? [];
    const areAllContinuous = priceRows.every((priceList, index) => {
      const previousUpper = priceRows[index - 1]?.maxIncludedCount;
      return !!previousUpper ? previousUpper < priceRows[index]?.maxIncludedCount : true;
    });

    return areAllContinuous ? null : { [PriceGridValidationError.BoundsContinuity]: true };
  }
}
