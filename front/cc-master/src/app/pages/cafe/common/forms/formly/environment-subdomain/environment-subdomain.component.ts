import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-environment-domain',
  templateUrl: './environment-subdomain.component.html',
})
export class FormlyFieldEnvironmentSubdomain extends FieldType {
  readonly formControl: FormControl;
}
