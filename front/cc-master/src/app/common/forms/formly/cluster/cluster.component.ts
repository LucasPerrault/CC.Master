import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { FieldType } from '@ngx-formly/core';

@Component({
  selector: 'cc-formly-field-cluster',
  templateUrl: './cluster.component.html',
})
export class FormlyFieldCluster extends FieldType {
  readonly formControl: FormControl;
}
