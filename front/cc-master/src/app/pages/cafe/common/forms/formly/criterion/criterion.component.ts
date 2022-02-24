import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-criterion',
  templateUrl: './criterion.component.html',
})
export class FormlyFieldCriterion extends FieldType {
  readonly formControl: FormControl;
}
