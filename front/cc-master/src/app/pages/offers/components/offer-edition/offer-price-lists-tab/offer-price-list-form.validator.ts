import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { IPriceList } from '@cc/domain/billing/offers';
import { isEqual } from 'date-fns';

export enum PriceListValidationError {
  UniqStartsOn = 'uniqStartsOn',
}

export class OfferPriceListFormValidator {
  public static uniqStartsOn(priceLists: IPriceList[]): ValidatorFn {
    return (control: AbstractControl): ValidationErrors => {
      const startsOn = !!control.value ? new Date(control.value) : null;
      const isUniq = priceLists.every(p => !isEqual(new Date(p.startsOn), startsOn));
      return !isUniq ? { [PriceListValidationError.UniqStartsOn]: true } : null;
    };
  }
}
