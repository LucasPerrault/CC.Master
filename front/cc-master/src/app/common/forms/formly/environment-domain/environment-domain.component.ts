import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-environment-domain',
  templateUrl: './environment-domain.component.html',
})
export class FormlyFieldEnvironmentDomain extends FieldType {
  readonly formControl: FormControl;
}
