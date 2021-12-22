import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-distributor',
  templateUrl: './distributor.component.html',
})
export class FormlyFieldDistributor extends FieldType {
  readonly formControl: FormControl;
}
