import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-facet',
  templateUrl: './facet.component.html',
})
export class FormlyFieldFacet extends FieldType {
  readonly formControl: FormControl;
}
