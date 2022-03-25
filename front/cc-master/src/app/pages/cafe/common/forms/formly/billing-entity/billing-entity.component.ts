import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-billing-entity',
  templateUrl: './billing-entity.component.html',
})
export class FormlyFieldBillingEntity extends FieldType {
  readonly formControl: FormControl;
}
