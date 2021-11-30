import { AbstractControl, FormControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { isEqual, isFirstDayOfMonth } from 'date-fns';

import { IPriceListForm, IPriceRowForm } from '../models/price-list-form.interface';

export enum PriceListValidationError {
  UniqStartsOn = 'uniqStartsOn',
  BoundsContinuity = 'boundsContinuity',
  StartsOnFirstDayOfTheMonth = 'startsOnFirstDayOfTheMonth',
  Required = 'required',
}

export class PriceListsValidators {
  public static boundsContinuity(control: FormControl): ValidationErrors {
    const priceList: IPriceListForm = control.value;
    return !PriceListsValidators.areContinuousBounds(priceList?.rows ?? [])
      ? { [PriceListValidationError.BoundsContinuity]: true }
      : null;
  }

  public static boundsContinuityRange(control: FormControl): ValidationErrors {
    const priceLists: IPriceListForm[] = control.value ?? [];
    const allHaveContinuousBounds = priceLists.every(priceList => PriceListsValidators.areContinuousBounds(priceList.rows));
    return !allHaveContinuousBounds ? { [PriceListValidationError.BoundsContinuity]: true } : null;
  }

  public static uniqStartsOn(startDates: Date[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors => {
      const priceList: IPriceListForm = control.value;
      const startDate = !!priceList?.startsOn ? new Date(priceList?.startsOn) : null;

      return !PriceListsValidators.hasUniqStartDate(startDates, startDate)
        ? { [PriceListValidationError.UniqStartsOn]: true } : null;
    };
  }

  public static uniqStartsOnRange(control: FormControl): ValidationErrors {
    const priceLists: IPriceListForm[] = control.value ?? [];

    const allHaveUniqStartDate = priceLists.every((priceList, index) => {
      const startDatesToCompare = priceLists
        .filter((p, i) => i !== index)
        .map(p => new Date(p.startsOn));

      return PriceListsValidators.hasUniqStartDate(startDatesToCompare, new Date(priceList.startsOn));
    });

    return !allHaveUniqStartDate ? { [PriceListValidationError.UniqStartsOn]: true } : null;
  }

  public static startsOnFirstDayOfTheMonthRange(control: FormControl): ValidationErrors {
    const priceLists: IPriceListForm[] = control.value;

    const allStartedOnFirstDay = priceLists.every(priceList => PriceListsValidators.isStartedOnFirstDayOfTheMonth(priceList));

    return !allStartedOnFirstDay
      ? { [PriceListValidationError.StartsOnFirstDayOfTheMonth]: true }
      : null;
  }

  public static startsOnFirstDayOfTheMonth(control: FormControl): ValidationErrors {
    const priceList: IPriceListForm = control.value;

    return !PriceListsValidators.isStartedOnFirstDayOfTheMonth(priceList)
      ? { [PriceListValidationError.StartsOnFirstDayOfTheMonth]: true }
      : null;
  }

  private static isStartedOnFirstDayOfTheMonth(priceList: IPriceListForm): boolean {
    const startDate = !!priceList.startsOn ? new Date(priceList.startsOn) : null;
    return !!startDate && isFirstDayOfMonth(startDate);
  }

  private static areContinuousBounds(priceRows: IPriceRowForm[]): boolean {
    return priceRows.every((priceList, index) => {
      const previousUpper = priceRows[index - 1]?.maxIncludedCount;
      return !!previousUpper ? previousUpper < priceRows[index]?.maxIncludedCount : true;
    });
  }

  private static hasUniqStartDate(startDates: Date[], startDate: Date): boolean {
    return startDates.every(date => !isEqual(date, startDate));
  }
}
