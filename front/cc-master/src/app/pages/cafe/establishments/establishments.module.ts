import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { TranslateModule } from '@cc/aspects/translate';
import { PagingModule } from '@cc/common/paging';

import { EstablishmentListComponent } from './components/establishment-list/establishment-list.component';
import { EstablishmentsComponent } from './establishments.component';
import { EstablishmentsConfiguration } from './establishments.configuration';
import { EstablishmentsDataService } from './services/establishments-data.service';

@NgModule({
  declarations: [EstablishmentsComponent, EstablishmentListComponent],
  imports: [
    CommonModule,
    TranslateModule,
    PagingModule,
  ],
  exports: [EstablishmentsComponent],
  providers: [EstablishmentsConfiguration, EstablishmentsDataService],
})
export class EstablishmentsModule { }
