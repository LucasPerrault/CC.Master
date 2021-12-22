import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-application',
  templateUrl: './application.component.html',
})
export class FormlyFieldApplication extends FieldType {
  readonly formControl: FormControl;
}
