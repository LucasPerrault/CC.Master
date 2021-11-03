import { FormControl, ValidationErrors } from '@angular/forms';

import { IEditablePriceGrid } from './editable-price-grid.interface';

export enum PriceGridValidationError {
  ClosestBounds = 'closestBounds',
  BoundsContinuity = 'boundsContinuity',
  MinBound = 'minBound',
  Required = 'required',
}

export class EditablePriceGridValidators {
  public static upperBoundSuperior(control: FormControl): ValidationErrors {
    const priceList: IEditablePriceGrid = control.value;
    if (!priceList.lowerBound || !priceList.upperBound || priceList.lowerBound <= priceList.upperBound) {
      return null;
    }

    return { [PriceGridValidationError.ClosestBounds]: true };
  }

  public static minimumZero(control: FormControl): ValidationErrors {
    const priceLists: IEditablePriceGrid[] = control.value.priceLists || [];
    const lowerBounds = priceLists.map(p => p.lowerBound);
    const minLowerBound = Math.min(...lowerBounds);

    return minLowerBound === 0 ? null : { [PriceGridValidationError.MinBound]: true };
  }

  public static boundsContinuity(control: FormControl): ValidationErrors {
    const priceLists: IEditablePriceGrid[] = control.value.priceLists || [];
    const areAllContinuous = priceLists.every((priceList, index) => {
      if (priceList.lowerBound === 0) {
        return true;
      }

      const previousUpper = priceLists[index - 1]?.upperBound;
      return priceList.lowerBound ===  previousUpper + 1;
    });

    return areAllContinuous ? null : { [PriceGridValidationError.BoundsContinuity]: true };
  }
}
