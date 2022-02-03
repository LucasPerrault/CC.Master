import { Component, forwardRef } from '@angular/core';
import { NG_VALIDATORS, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ALuApiService } from '@lucca-front/ng/api';

import { FacetsAndColumnsApiSelectComponent } from '../facets-and-columns-api-select.component';
import { EnvironmentFacetsAndColumnsApiSelectService } from './environment-facets-and-columns-api-select.service';

@Component({
  selector: 'cc-environment-facets-and-columns-api-select',
  templateUrl: './../facets-and-columns-api-select.component.html',
  styleUrls: ['./../facets-and-columns-api-select.component.scss'],
  providers: [
    {
      provide: ALuApiService,
      useClass: EnvironmentFacetsAndColumnsApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EnvironmentFacetsAndColumnsApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: EnvironmentFacetsAndColumnsApiSelectComponent,
    },
  ],
})
export class EnvironmentFacetsAndColumnsApiSelectComponent extends FacetsAndColumnsApiSelectComponent {}
