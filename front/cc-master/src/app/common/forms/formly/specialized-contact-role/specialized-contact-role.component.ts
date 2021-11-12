import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-specialized-contact-role',
  templateUrl: './specialized-contact-role.component.html',
})
export class FormlyFieldSpecializedContactRole extends FieldType {
  readonly formControl: FormControl;
}
