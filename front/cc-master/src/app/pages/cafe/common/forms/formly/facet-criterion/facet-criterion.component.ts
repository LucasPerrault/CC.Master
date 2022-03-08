import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-facet-criterion',
  templateUrl: './facet-criterion.component.html',
})
export class FormlyFieldFacetCriterion extends FieldType {
  readonly formControl: FormControl;
}
