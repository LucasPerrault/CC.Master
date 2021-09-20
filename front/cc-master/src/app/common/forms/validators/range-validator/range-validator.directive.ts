import { Directive, Input } from '@angular/core';
import { AbstractControl, ValidationErrors, Validator } from '@angular/forms';

import { IRange } from './range.interface';
import { rangeValidatorFn } from './range-validator.constant';

@Directive({
  // eslint-disable-next-line @angular-eslint/directive-selector
  selector: '[range]',
})
export class RangeValidatorDirective implements Validator {
  @Input() public range: IRange;

  validate(control: AbstractControl): ValidationErrors | null {
    return rangeValidatorFn(this.range)(control);
  }
}

