import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-environment-domain',
  templateUrl: './country.component.html',
})
export class FormlyFieldCountry extends FieldType {
  readonly formControl: FormControl;
}
