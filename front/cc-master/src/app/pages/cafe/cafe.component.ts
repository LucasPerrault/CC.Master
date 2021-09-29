import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';

import { CafeConfiguration } from './cafe-configuration';
import { ICafeConfiguration } from './cafe-configuration.interface';

@Component({
  selector: 'cc-cafe',
  templateUrl: './cafe.component.html',
})
export class CafeComponent {
  public cafeFilters: FormControl = new FormControl();
  public configuration: ICafeConfiguration;

  constructor(cafeConfiguration: CafeConfiguration) {
    this.configuration = cafeConfiguration;
  }
}
