import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';
import { LuApiModule } from '@lucca-front/ng/api';
import { LuInputModule } from '@lucca-front/ng/input';
import {
  LuOptionModule,
  LuOptionPagerModule,
  LuOptionPickerModule,
  LuOptionSearcherModule,
} from '@lucca-front/ng/option';
import { LuSelectInputModule } from '@lucca-front/ng/select';
import { FormlyModule } from '@ngx-formly/core';

import { EnvironmentFacetsAndColumnsApiSelectComponent } from './environment/environment-facets-and-columns-api-select.component';
import { EstablishmentFacetsAndColumnsApiSelectComponent } from './establishment/establishment-facets-and-columns-api-select.component';
import { FacetsAndColumnsApiSelectComponent } from './facets-and-columns-api-select.component';
import { FacetsAndColumnsApiSelectService } from './facets-and-columns-api-select.service';

@NgModule({
  declarations: [
    EnvironmentFacetsAndColumnsApiSelectComponent,
    EstablishmentFacetsAndColumnsApiSelectComponent,
    FacetsAndColumnsApiSelectComponent,
  ],
  imports: [
    FormsModule,
    LuSelectInputModule,
    LuOptionPickerModule,
    LuOptionSearcherModule,
    LuOptionPagerModule,
    LuOptionModule,
    LuInputModule,
    LuApiModule,
    TranslateModule,
    CommonModule,
    ReactiveFormsModule,
    FormlyModule,
    PagingModule,
  ],
  exports: [
    EnvironmentFacetsAndColumnsApiSelectComponent,
    EstablishmentFacetsAndColumnsApiSelectComponent,
    FacetsAndColumnsApiSelectComponent,
  ],
  providers: [
    FacetsAndColumnsApiSelectService,
  ],
})
export class FacetsAndColumnsApiSelectModule { }
