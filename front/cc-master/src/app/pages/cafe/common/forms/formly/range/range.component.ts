import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-range',
  templateUrl: './range.component.html',
})
export class FormlyFieldRange extends FieldType {
  readonly formControl: FormControl;
}
