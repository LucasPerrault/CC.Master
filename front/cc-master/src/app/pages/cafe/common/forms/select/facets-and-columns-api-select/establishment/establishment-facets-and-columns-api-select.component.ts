import { Component, forwardRef } from '@angular/core';
import { NG_VALIDATORS, NG_VALUE_ACCESSOR } from '@angular/forms';
import { ALuApiService } from '@lucca-front/ng/api';

import { FacetsAndColumnsApiSelectComponent } from '../facets-and-columns-api-select.component';
import { EstablishmentFacetsAndColumnsApiSelectService } from './establishment-facets-and-columns-api-select.service';

@Component({
  selector: 'cc-establishment-facets-and-columns-api-select',
  templateUrl: './../facets-and-columns-api-select.component.html',
  styleUrls: ['./../facets-and-columns-api-select.component.scss'],
  providers: [
    {
      provide: ALuApiService,
      useClass: EstablishmentFacetsAndColumnsApiSelectService,
    },
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => EstablishmentFacetsAndColumnsApiSelectComponent),
      multi: true,
    },
    {
      provide: NG_VALIDATORS,
      multi: true,
      useExisting: EstablishmentFacetsAndColumnsApiSelectComponent,
    },
  ],
})
export class EstablishmentFacetsAndColumnsApiSelectComponent extends FacetsAndColumnsApiSelectComponent {}
