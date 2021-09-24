import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { CafeInstancesComponent } from './cafe-instances.component';
import { CountryListModalModule } from './components/country-list-modal/country-list-modal.module';
import { EnvironmentListComponent } from './components/environment-list/environment-list.component';
import { EnvironmentDataService } from './services/environment-data.service';
import { EnvironmentListService } from './services/environment-list.service';
import { EnvironmentAdditionalColumnSelectModule } from './components/environment-additional-column-select/environment-additional-column-select.module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    CafeInstancesComponent,
    EnvironmentListComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
    CountryListModalModule,
    EnvironmentAdditionalColumnSelectModule,
    ReactiveFormsModule,
  ],
  exports: [
    CafeInstancesComponent,
  ],
  providers: [
    EnvironmentListService,
    EnvironmentDataService,
  ],
})
export class CafeInstancesModule { }
