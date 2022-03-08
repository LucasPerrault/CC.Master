import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-facet-value',
  templateUrl: './facet-value.component.html',
})
export class FormlyFieldFacetValue extends FieldType {
  readonly formControl: FormControl;
}
